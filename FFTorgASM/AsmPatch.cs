﻿using System;
using System.Collections.Generic;
using PatcherLib.Datatypes;
using PatcherLib.Utilities;

namespace FFTorgASM
{
    class FileAsmPatch : AsmPatch
    {
        private static System.Windows.Forms.OpenFileDialog ofd;

        private static System.Windows.Forms.OpenFileDialog OpenFileDialog
        {
            get
            {
                if ( ofd == null )
                {
                    ofd = new System.Windows.Forms.OpenFileDialog();
                    ofd.CheckFileExists = true;
                    ofd.CheckPathExists = true;
                    ofd.FileName = string.Empty;
                    ofd.Filter = "All files (*.*)|*.*";
                    ofd.Multiselect = false;
                    ofd.ShowHelp = false;
                    ofd.ShowReadOnly = true;
                }
                return ofd;
            }
                
        }

        private InputFilePatch patch;
        public FileAsmPatch( string name, string filename, string description, InputFilePatch patch )
            : base( name, filename, description, new PatchedByteArray[] { patch }, false, false )
        {
            this.patch = patch;
        }

        public void SetFilename( string filename )
        {
            patch.SetFilename( filename );
        }

        public override bool ValidatePatch()
        {
            bool result = false;
            System.Windows.Forms.MethodInvoker mi = delegate()
            {
                if ( !string.IsNullOrEmpty( patch.Filename ) )
                {
                    OpenFileDialog.InitialDirectory = System.IO.Path.GetDirectoryName( patch.Filename );
                    OpenFileDialog.FileName = System.IO.Path.GetFileName( patch.Filename );
                }

                if ( OpenFileDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK )
                {
                    try
                    {
                        SetFilename( OpenFileDialog.FileName );
                        result = true;
                    }
                    catch
                    {
                        result = false;
                    }
                }
                else
                {
                    result = false;
                }
            };

            mi();

            return result;
        }

    }

    public struct VariableType
    {
    	public byte numBytes;
        public byte[] byteArray;
        public string name;
        public List<PatchedByteArray> content;
        public bool isReference;
        public VariableReference reference;
    }

    public struct VariableReference
    {
        public string name;
        public string operatorSymbol;
        public uint operand;
    }

    public class AsmPatch : IList<PatchedByteArray>
    {
        List<PatchedByteArray> innerList;
        List<PatchedByteArray> varInnerList;

        public string Name { get; private set; }
        public string Description { get; private set; }

        public string Filename { get; private set; }

        public IList<VariableType> _variables; 
        public IList<VariableType> Variables
        {
            get
            {
                return _variables;
            }
            private set
            {
                _variables = value;
                CreateVariableMap();
            }
        }
        
        public Dictionary<string, VariableType> VariableMap { get; private set; }

        public List<ASMEncoding.Helpers.BlockMove> blockMoveList { get; set; }

        private IEnumerator<PatchedByteArray> enumerator;

        public bool HideInDefault { get; private set; }
        public bool IsHidden { get; private set; }

        public string ErrorText { get; set; }

        public virtual bool ValidatePatch()
        {
            return true;
        }

        public AsmPatch( string name, string filename, string description, IEnumerable<PatchedByteArray> patches, bool hideInDefault, bool isHidden )
        {
            enumerator = new AsmPatchEnumerator( this );
            this.Name = name;
            this.Filename = filename;
            Description = description;
            innerList = new List<PatchedByteArray>( patches );
            Variables = new VariableType[0];
            varInnerList = new List<PatchedByteArray>();
            blockMoveList = new List<ASMEncoding.Helpers.BlockMove>();
            this.HideInDefault = hideInDefault;
            this.IsHidden = isHidden;
        }

        public AsmPatch(string name, string filename, string description, IEnumerable<PatchedByteArray> patches, bool hideInDefault, bool isHidden, IList<VariableType> variables)
            : this( name, filename, description, patches, hideInDefault, isHidden )
        {
        	VariableType[] myVars = new VariableType[variables.Count];
            variables.CopyTo( myVars, 0 );
            Variables = myVars;
            SetVarInnerList();
        }

        private void SetVarInnerList()
        {
            varInnerList.Clear();
            foreach (VariableType varType in Variables)
            {
                List<PatchedByteArray> patchedByteArrayList = varType.content;
                if (patchedByteArrayList != null)
                {
                    foreach (PatchedByteArray patchedByteArray in patchedByteArrayList)
                    {
                        varInnerList.Add(patchedByteArray);
                    }
                }
            }
        }

        private void CreateVariableMap()
        {
            VariableMap = new Dictionary<string, VariableType>();
            foreach (VariableType variable in Variables)
            {
                string name = variable.name;
                if (!VariableMap.ContainsKey(name))
                {
                    VariableMap.Add(name, variable);
                }
            }
        }

        public int CountNonReferenceVariables()
        {
            int count = 0;
            foreach (VariableType variable in Variables)
            {
                if (!variable.isReference)
                    count++;
            }
            return count;
        }

        public int IndexOf( PatchedByteArray item )
        {
            return innerList.IndexOf( item );
        }

        public void Insert( int index, PatchedByteArray item )
        {
            throw new NotImplementedException();
        }

        public void RemoveAt( int index )
        {
            throw new InvalidOperationException( "collection is readonly" );
        }

        public PatchedByteArray this[int index]
        {
            get
            {
                if ( index < innerList.Count )
                {
                    return innerList[index];
                }
                else
                {
                    return varInnerList[index - innerList.Count];
                }
            }
            set
            {
                throw new InvalidOperationException( "collection is readonly" );
            }
        }

        public void Add( PatchedByteArray item )
        {
            throw new InvalidOperationException( "collection is readonly" );
        }

        public void Clear()
        {
            throw new InvalidOperationException( "collection is readonly" );
        }

        public bool Contains( PatchedByteArray item )
        {
            return innerList.Contains( item );
        }

        public void CopyTo( PatchedByteArray[] array, int arrayIndex )
        {
            innerList.CopyTo( array, arrayIndex );
        }

        public int Count
        {
            get { return innerList.Count + varInnerList.Count; }
        }

        public int NonVariableCount
        {
            get { return innerList.Count; }
        }

        public bool IsReadOnly
        {
            get { return true; }
        }

        public bool Remove( PatchedByteArray item )
        {
            throw new InvalidOperationException( "collection is readonly" );
        }

        public IEnumerator<PatchedByteArray> GetEnumerator()
        {
            enumerator.Reset();
            return enumerator;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            enumerator.Reset();
            return enumerator as System.Collections.IEnumerator;
        }

        public override string ToString()
        {
            return Name;
        }

        public void Update(ASMEncoding.ASMEncodingUtility asmUtility)
        {
            UpdateReferenceVariableValues();

            List<PatchedByteArray> allPatches = GetAllPatches();
            foreach (PatchedByteArray patchedByteArray in allPatches)
            {
                if (patchedByteArray.IsAsm)
                {
                    string encodeContent = patchedByteArray.AsmText;
                    //string strPrefix = "";
                    //IList<VariableType> variables = Variables;

                    System.Text.StringBuilder sbPrefix = new System.Text.StringBuilder();
                    foreach (PatchedByteArray currentPatchedByteArray in allPatches)
                    {
                        if (!string.IsNullOrEmpty(currentPatchedByteArray.Label))
                            sbPrefix.AppendFormat(".label @{0}, {1}{2}", currentPatchedByteArray.Label, currentPatchedByteArray.RamOffset, Environment.NewLine);
                    }
                    foreach (VariableType variable in Variables)
                    {
                        sbPrefix.AppendFormat(".eqv %{0}, {1}{2}", ASMEncoding.Helpers.ASMStringHelper.RemoveSpaces(variable.name).Replace(",", ""),
                            AsmPatch.GetUnsignedByteArrayValue_LittleEndian(variable.byteArray), Environment.NewLine);
                    }

                    encodeContent = sbPrefix.ToString() + patchedByteArray.AsmText;
                    //patchedByteArray.SetBytes(asmUtility.EncodeASM(encodeContent, (uint)patchedByteArray.RamOffset).EncodedBytes);

                    byte[] bytes = asmUtility.EncodeASM(encodeContent, (uint)patchedByteArray.RamOffset).EncodedBytes;

                    if ((!patchedByteArray.IsMoveSimple) && (blockMoveList.Count > 0))
                    {
                        bytes = asmUtility.UpdateBlockReferences(bytes, (uint)patchedByteArray.RamOffset, true, blockMoveList);
                    }

                    patchedByteArray.SetBytes(bytes);
                }
            }
        }

        private void UpdateReferenceVariableValues()
        {
            foreach (VariableType variable in Variables)
            {
                if (variable.isReference)
                    UpdateReferenceVariableValue(variable);
            }
        }

        private void UpdateReferenceVariableValue(VariableType variable)
        {
            if (variable.isReference)
            {
                byte[] referenceBytes = VariableMap[variable.reference.name].byteArray;
                uint value = AsmPatch.GetUnsignedByteArrayValue_LittleEndian(referenceBytes);

                switch (variable.reference.operatorSymbol)
                {
                    case "+":
                        value += variable.reference.operand;
                        break;
                    case "-":
                        value -= variable.reference.operand;
                        break;
                    case "*":
                        value *= variable.reference.operand;
                        break;
                    case "/":
                        value /= variable.reference.operand;
                        break;
                }

                UpdateVariable(variable, value);
            }
        }

        public static void UpdateVariable(VariableType variable, UInt32 newValue)
        {
            for (int i = 0; i < variable.numBytes; i++)
            {
                byte byteValue = (byte)((newValue >> (i * 8)) & 0xff);
                variable.byteArray[i] = byteValue;
                foreach (PatchedByteArray patchedByteArray in variable.content)
                {
                    patchedByteArray.GetBytes()[i] = byteValue;
                }
            }
        }

        public static VariableType CopyVariable(VariableType variable)
        {
            VariableType resultVariable = new VariableType();

            resultVariable.numBytes = variable.numBytes;
            resultVariable.byteArray = (byte[])(variable.byteArray.Clone());
            resultVariable.name = variable.name;
            resultVariable.isReference = variable.isReference;
            resultVariable.reference = variable.reference;

            resultVariable.content = new List<PatchedByteArray>();
            foreach (PatchedByteArray patchedByteArray in variable.content)
            {
                resultVariable.content.Add(patchedByteArray.Copy());
            }

            return resultVariable;
        }

        public List<PatchedByteArray> GetAllPatches()
        {
            List<PatchedByteArray> allPatches = new List<PatchedByteArray>(Count);
            allPatches.AddRange(innerList);
            allPatches.AddRange(varInnerList);
            return allPatches;
        }

        // Get patched byte array list with sequential patches combined into the same patch.
        public List<PatchedByteArray> GetCombinedPatchList()
        {
            List<PatchedByteArray> resultList = new List<PatchedByteArray>();
            bool isSequential = false;

            if (innerList.Count > 0)
            {
                PatchedByteArray currentPatch = null;
                List<byte> bytes = null;
                //foreach (PatchedByteArray patchedByteArray in innerList)
                for (int index = 0; index < innerList.Count; index++)
                {
                    PatchedByteArray patchedByteArray = innerList[index];

                    if (patchedByteArray is InputFilePatch)
                        continue;

                    isSequential = patchedByteArray.IsSequentialOffset;

                    if (currentPatch == null)
                    {
                        currentPatch = patchedByteArray.Copy();
                        bytes = new List<byte>(currentPatch.GetBytes());
                    }
                    else if (isSequential)
                    {
                        //List<byte> bytes = new List<byte>(currentPatch.GetBytes());
                        bytes.AddRange(patchedByteArray.GetBytes());
                        //currentPatch.SetBytes(bytes.ToArray());
                    }
                    else
                    {
                        currentPatch.SetBytes(bytes.ToArray());
                        resultList.Add(currentPatch);
                        currentPatch = patchedByteArray.Copy();
                        bytes = new List<byte>(currentPatch.GetBytes());
                    }
                }

                //if (isSequential)
                if (bytes != null)
                {
                    currentPatch.SetBytes(bytes.ToArray());
                    resultList.Add(currentPatch);
                }
            }

            resultList.AddRange(varInnerList);

            return resultList;
        }

        public void MoveBlock(ASMEncoding.ASMEncodingUtility utility, MovePatchRange movePatchRange)
        {
            MoveBlocks(utility, new MovePatchRange[1] { movePatchRange });
        }

        public void MoveBlocks(ASMEncoding.ASMEncodingUtility utility, IEnumerable<MovePatchRange> movePatchRanges)
        {
            /*
            Dictionary<PatchRange, bool> isSequentialAdd = new Dictionary<PatchRange, bool>();
            foreach (KeyValuePair<PatchRange, uint> blockMove in blockMoves)
            {
                isSequentialAdd[blockMove.Key] = false;
            }
            */
            List<ASMEncoding.Helpers.BlockMove> blockMoves = new List<ASMEncoding.Helpers.BlockMove>();
            foreach (MovePatchRange patchRange in movePatchRanges)
            {
                //PatchRange patchRange = movePair.Key;
                uint fileToRamOffset = PatcherLib.Iso.PsxIso.GetRamOffset(patchRange.Sector, true);
                ASMEncoding.Helpers.BlockMove blockMove = new ASMEncoding.Helpers.BlockMove();
                blockMove.Location = (uint)patchRange.StartOffset + fileToRamOffset;
                blockMove.EndLocation = (uint)patchRange.EndOffset + fileToRamOffset;
                blockMove.Offset = patchRange.MoveOffset;
                blockMoves.Add(blockMove);
            }

            List<PatchedByteArray> allPatchList = GetAllPatches();
            //foreach (PatchedByteArray patchedByteArray in innerList)
            foreach (PatchedByteArray patchedByteArray in allPatchList)
            {
                if (patchedByteArray is InputFilePatch)
                    continue;

                byte[] bytes = patchedByteArray.GetBytes();

                foreach (MovePatchRange patchRange in movePatchRanges)
                {
                    //PatchRange patchRange = movePair.Key;
                    //uint offset = movePair.Value;

                    //if ((patchedByteArray.RamOffset == blockMove.Location) && ((patchedByteArray.RamOffset + bytes.Length) == blockMove.EndLocation))
                    //if (patchedByteArray.RamOffset == blockMove.Location)
                    //PatchRange patchRange = new PatchRange(patchedByteArray);
                    //PatchRange patchRange2 = new PatchRange(
                    //if (
                    //if (blockMove.Key
                    if (patchRange.HasOverlap(patchedByteArray))
                    {
                        patchedByteArray.Offset += patchRange.MoveOffset; // offset;
                        patchedByteArray.RamOffset += patchRange.MoveOffset; // offset;
                        //isSequentialAdd[patchRange] = true;
                    }
                    /*
                    else if ((isSequentialAdd[patchRange]) && (patchedByteArray.IsSequentialOffset))
                    {
                        patchedByteArray.Offset += offset;
                        patchedByteArray.RamOffset += offset;
                        //patchRange.EndOffset += (uint)bytes.Length;
                    }
                    else
                    {
                        isSequentialAdd[patchRange] = false;
                    }
                    */
                }

                if ((patchedByteArray.IsCheckedAsm) && (!patchedByteArray.IsMoveSimple))
                {
                    byte[] newBytes = utility.UpdateBlockReferences(bytes, (uint)patchedByteArray.RamOffset, true, blockMoves);
                    patchedByteArray.SetBytes(newBytes);
                }
            }

            foreach (ASMEncoding.Helpers.BlockMove blockMove in blockMoves)
            {
                bool isAlreadyPresent = false;
                foreach (ASMEncoding.Helpers.BlockMove listBlockMove in blockMoveList)
                {
                    if (blockMove.IsEqual(listBlockMove))
                    {
                        isAlreadyPresent = true;
                        break;
                    }
                }

                if (!isAlreadyPresent)
                {
                    blockMoveList.Add(blockMove);
                }
            }
        }

        public string CreateXML()
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();
            sb.AppendFormat("    <Patch name=\"{0}\">{1}", Name, Environment.NewLine);

            if (!string.IsNullOrEmpty(Description))
            {
                sb.AppendFormat("        <Description>{0}</Description>{1}", Description, Environment.NewLine);
            }

            sb.Append(PatcherLib.Utilities.Utilities.CreatePatchXML(innerList));

            foreach (VariableType variable in Variables)
            {
                int value = variable.byteArray.ToIntLE();
                System.Text.StringBuilder sbSpecific = new System.Text.StringBuilder();
                int patchCount = variable.content.Count;

                for (int index = 0; index < patchCount; index++)
                {
                    PatchedByteArray patchedByteArray = variable.content[index];
                    //string file = Enum.GetName(typeof(PatcherLib.Iso.PsxIso.Sectors), patchedByteArray.Sector);
                    string file = PatcherLib.Iso.PsxIso.GetSectorName(patchedByteArray.Sector);
                    sbSpecific.AppendFormat("{0}:{1}{2}", file, patchedByteArray.Offset.ToString("X"), ((index < (patchCount - 1)) ? "," : ""));
                }

                string strVariableReference = "";
                if (variable.isReference)
                {
                    strVariableReference = String.Format(" reference=\"{0}\" operator=\"{1}\" operand=\"{2}\"", variable.reference.name, variable.reference.operand, variable.reference.operatorSymbol);
                }

                string strDefault = variable.isReference ? "" : String.Format(" default=\"{0}\"", value.ToString("X"));

                string strSpecific = sbSpecific.ToString();
                string strContent = string.IsNullOrEmpty(strSpecific) ? "symbol=\"true\"" : String.Format("specific=\"{0}\"", strSpecific);

                sb.AppendFormat("        <Variable name=\"{0}\" {1} bytes=\"{2}\"{3}{4} />{5}",
                    variable.name, strContent, variable.numBytes, strDefault, strVariableReference, Environment.NewLine);
            }

            sb.AppendLine("    </Patch>");
            return sb.ToString();
        }

		// Returns combined value of byte array (little endian)
		public static UInt32 GetUnsignedByteArrayValue_LittleEndian(Byte[] bytes)
        {
			UInt32 result = 0;
			int i = 0;
			foreach (Byte currentByte in bytes)
			{
				result = result | ((uint)(currentByte << (i * 8)));
				i++;
			}
			return result;
        }

        private class AsmPatchEnumerator : IEnumerator<PatchedByteArray>
        {
            private int index = -1;
            private AsmPatch owner;
            public AsmPatchEnumerator( AsmPatch owner )
            {
                this.owner = owner;
            }
            #region IEnumerator<PatchedByteArray> Members

            public PatchedByteArray Current
            {
                get { return owner[index]; }
            }

            #endregion


            #region IDisposable Members

            public void Dispose()
            {
            }

            #endregion

            #region IEnumerator Members

            object System.Collections.IEnumerator.Current
            {
                get { return Current; }
            }

            public bool MoveNext()
            {
                index++;
                return index < owner.Count;
            }

            public void Reset()
            {
                index = -1;
            }

            #endregion
        }
    }
}
