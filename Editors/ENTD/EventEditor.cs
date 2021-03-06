/*
    Copyright 2007, Joe Davidson <joedavidson@gmail.com>

    This file is part of FFTPatcher.

    FFTPatcher is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    FFTPatcher is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with FFTPatcher.  If not, see <http://www.gnu.org/licenses/>.
*/

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Collections.Generic;
using FFTPatcher.Datatypes;
using PatcherLib.Datatypes;

namespace FFTPatcher.Editors
{
    public partial class EventEditor : BaseEditor
    {
		#region Instance Variables

        private int[] columnWidths = new int[7] { 125, 125, 100, 75, 50, 90, 50 };
        private int[] cumulativeWidths = new int[7] { 125, 250, 350, 425, 475, 565, 615 };
        private Event evt;
        private Context ourContext = Context.Default;
        private bool[] _isPositionOccupied;

		#endregion Instance Variables 

        private ToolTip toolTip;
        public ToolTip ToolTip
        {
            set
            {
                toolTip = value;
                eventUnitEditor.ToolTip = value;
            }
        }

		#region Public Properties

        public EventUnit ClipBoardUnit { get; private set; }

        public Event Event
        {
            get { return evt; }
            set
            {
                SetEvent(value, ourContext);
            }
        }

		#endregion Public Properties 

		#region Constructors (1) 

        public EventEditor()
        {
            InitializeComponent();
            unitSelectorListBox.SelectedIndexChanged += unitSelectorComboBox_SelectedIndexChanged;
            eventUnitEditor.DataChanged += eventUnitEditor_DataChanged;
            unitSelectorListBox.DrawItem += unitSelectorListBox_DrawItem;
            unitSelectorListBox.DrawMode = DrawMode.OwnerDrawFixed;
            unitSelectorListBox.ContextMenu = new ContextMenu( 
                new MenuItem[] { new MenuItem( "Clone", CopyClickEventHandler ), new MenuItem( "Paste clone", PasteClickEventHandler ) } );
            unitSelectorListBox.ContextMenu.MenuItems[1].Enabled = false;
            unitSelectorListBox.MouseDown += new MouseEventHandler( unitSelectorListBox_MouseDown );
        }

		#endregion Constructors 

		#region Public Methods

        public void SetEvent(Event value, Context context)
        {
            if (value == null)
            {
                evt = null;
                Enabled = false;
            }
            else if (value != evt)
            {
                evt = value;
                UpdateView(context);
                Enabled = true;
            }
        }

        public void UpdateView(Context context)
        {
            if( ourContext != context )
            {
                ourContext = context;
                ClipBoardUnit = null;
                unitSelectorListBox.ContextMenu.MenuItems[1].Enabled = false;
            }

            eventUnitEditor.SuspendLayout();
            DetermineColumnWidths();
            unitSelectorListBox.DataSource = evt.Units;
            unitSelectorListBox.SelectedIndex = 0;
            //eventUnitEditor.EventUnit = unitSelectorListBox.SelectedItem as EventUnit;
            //eventUnitEditor.UpdateView(ourContext);
            eventUnitEditor.SetEventUnit(unitSelectorListBox.SelectedItem as EventUnit, context);
            eventUnitEditor.UpdateView(context);
            eventUnitEditor.ResumeLayout();
            eventUnitEditor_DataChanged( eventUnitEditor, EventArgs.Empty );
        }

		#endregion Public Methods 

		#region Private Methods (7) 

        private void CopyClickEventHandler( object sender, EventArgs args )
        {
            unitSelectorListBox.ContextMenu.MenuItems[1].Enabled = true;
            ClipBoardUnit = unitSelectorListBox.SelectedItem as EventUnit;
        }

        private void DetermineColumnWidths()
        {
            int maxSpriteWidth = 50;
            int maxNameWidth = 50;
            int maxJobWidth = 50;

            foreach( EventUnit unit in evt.Units )
            {
                string sprite = "*" + unit.SpriteSet.Name;
                string name = unit.SpecialName.Name;
                string job = unit.Job.Name;
                maxSpriteWidth = Math.Max( maxSpriteWidth, TextRenderer.MeasureText( sprite, unitSelectorListBox.Font ).Width );
                maxNameWidth = Math.Max( maxNameWidth, TextRenderer.MeasureText( name, unitSelectorListBox.Font ).Width );
                maxJobWidth = Math.Max( maxJobWidth, TextRenderer.MeasureText( job, unitSelectorListBox.Font ).Width );
            }

            columnWidths[0] = maxSpriteWidth + 10;
            columnWidths[1] = maxNameWidth + 10;
            columnWidths[2] = maxJobWidth + 10;
        }

        private void eventUnitEditor_DataChanged( object sender, System.EventArgs e )
        {
            CurrencyManager cm = (CurrencyManager)BindingContext[evt.Units];
            cm.Refresh();
            OnDataChanged( this, EventArgs.Empty );
        }

        private void PasteClickEventHandler( object sender, EventArgs args )
        {
            if( ClipBoardUnit != null )
            {
                ClipBoardUnit.CopyTo( unitSelectorListBox.SelectedItem as EventUnit );
                eventUnitEditor.EventUnit = unitSelectorListBox.SelectedItem as EventUnit;
                eventUnitEditor.UpdateView(ourContext);
                eventUnitEditor_DataChanged( eventUnitEditor, EventArgs.Empty );
            }
        }

        private void unitSelectorComboBox_SelectedIndexChanged( object sender, System.EventArgs e )
        {
            eventUnitEditor.EventUnit = unitSelectorListBox.SelectedItem as EventUnit;
        }

        private void unitSelectorListBox_DrawItem( object sender, DrawItemEventArgs e )
        {
         	// Find if units have overlapping positions
            _isPositionOccupied = new bool[evt.Units.Length];
            for (int index=0; index < evt.Units.Length; index++)
            	_isPositionOccupied[index] = false;
            
            for (int index=0; index < evt.Units.Length; index++)
            {
            	EventUnit eventUnit = evt.Units[index];
            	for (int innerIndex = index+1; innerIndex < evt.Units.Length; innerIndex++)
            	{
            		EventUnit innerEventUnit = evt.Units[innerIndex];
            		if ((eventUnit.X == innerEventUnit.X) && (eventUnit.Y == innerEventUnit.Y) && (eventUnit.UpperLevel == innerEventUnit.UpperLevel) 
                        && ((eventUnit.AlwaysPresent) && (innerEventUnit.AlwaysPresent)) 
                        && (eventUnit.SpriteSet.Value > 0) && (innerEventUnit.SpriteSet.Value > 0))
            		{
            			_isPositionOccupied[index] = true;
            			_isPositionOccupied[innerIndex] = true;
            		}
            	}
            }
        	
        	bool canCheckOccupied = (_isPositionOccupied != null);
        	if (canCheckOccupied)
        		canCheckOccupied = (_isPositionOccupied.Length == unitSelectorListBox.Items.Count);

            if( (e.Index > -1) && (e.Index < unitSelectorListBox.Items.Count) )
            {
                bool isSelected = ((e.State & DrawItemState.Selected) == DrawItemState.Selected);
                bool isOccupied = canCheckOccupied ? _isPositionOccupied[e.Index] : false;

                EventUnit unit = unitSelectorListBox.Items[e.Index] as EventUnit;
                bool unitExists = (unit.SpriteSet.Value > 0);

                int colorIndex = (isSelected ? 0 : (unit.AlwaysPresent ? 1 : 2));
                int teamIndex = (int)unit.TeamColor;
                FFTPatcher.Settings.CombinedColor teamColor = Settings.GetTeamColor(teamIndex, colorIndex);
                bool useConflictColor = (isOccupied && (!isSelected));
                bool useTeamColor = (unitExists && teamColor.UseColor);

                Color foreColor = (useConflictColor ? Color.Red : (useTeamColor ? teamColor.ForegroundColor : e.ForeColor));
                Color backColor = (useConflictColor ? Color.White : (useTeamColor ? teamColor.BackgroundColor : e.BackColor));
                string strPresentFlags = unit.AlwaysPresent ? (unit.RandomlyPresent ? "Always/Random" : "Always") : (unit.RandomlyPresent ? "Random" : "");

                using (Brush backBrush = new SolidBrush(backColor))
                {
                    e.Graphics.FillRectangle( backBrush, e.Bounds );

                    TextRenderer.DrawText(e.Graphics, (unit.HasChanged ? "*" : "") + unit.SpriteSet.Name, e.Font, new Point(e.Bounds.X + 0, e.Bounds.Y + 0), foreColor, TextFormatFlags.NoPrefix);
                    if (unitExists)
                    {
                        TextRenderer.DrawText(e.Graphics, unit.SpecialName.Name, e.Font, new Point(e.Bounds.X + cumulativeWidths[0], e.Bounds.Y + 0), foreColor, TextFormatFlags.NoPrefix);
                        TextRenderer.DrawText(e.Graphics, unit.Job.Name, e.Font, new Point(e.Bounds.X + cumulativeWidths[1], e.Bounds.Y + 0), foreColor, TextFormatFlags.NoPrefix);
                        TextRenderer.DrawText(e.Graphics, String.Format("({0}, {1}, {2})", unit.X, unit.Y, (unit.UpperLevel ? 1 : 0)), e.Font, new Point(e.Bounds.X + cumulativeWidths[2], e.Bounds.Y), foreColor);
                        TextRenderer.DrawText(e.Graphics, String.Format("0x{0:X2}", unit.UnitID), e.Font, new Point(e.Bounds.X + cumulativeWidths[3], e.Bounds.Y + 0), foreColor);
                        TextRenderer.DrawText(e.Graphics, strPresentFlags, e.Font, new Point(e.Bounds.X + cumulativeWidths[4], e.Bounds.Y + 0), foreColor);
                        TextRenderer.DrawText(e.Graphics, unit.TeamColor.ToString(), e.Font, new Point(e.Bounds.X + cumulativeWidths[5], e.Bounds.Y + 0), foreColor);
                    }
                    if( (e.State & DrawItemState.Focus) == DrawItemState.Focus )
                    {
                        e.DrawFocusRectangle();
                    }
                }
            }
        }

        void unitSelectorListBox_MouseDown( object sender, MouseEventArgs e )
        {
            if( e.Button == MouseButtons.Right )
            {
                unitSelectorListBox.SelectedIndex = unitSelectorListBox.IndexFromPoint( e.Location );
            }
        }

        private void unitSelectorListBox_KeyDown( object sender, KeyEventArgs args )
		{
			if (args.KeyCode == Keys.C && args.Control)
				CopyClickEventHandler( sender, args );
			else if (args.KeyCode == Keys.V && args.Control)
				PasteClickEventHandler( sender, args );
		}
        
		#endregion Private Methods 
    }
}
