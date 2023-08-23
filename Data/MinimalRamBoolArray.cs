using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GifFingTool.Data
{
    internal sealed class MinimalRamBoolArray : IEnumerable<bool>
    {
        private readonly int[] flagBlocks;


        public MinimalRamBoolArray(int flagBlockCount)
        {
            flagBlocks = new int[flagBlockCount];
        }

        public bool this[int index]
        {
            get
            {
                int blockIndex = index >> 5;
                int bitPosition = index & 0x1F;
                int result = flagBlocks[blockIndex] >> bitPosition;
                return (result & 0b1) > 0;
            }

            set
            {
                int blockIndex = index >> 5;
                int bitPosition = index & 0x1F;

                if (value)
                {
                    flagBlocks[blockIndex] |= 1 << bitPosition;
                }
                else
                {
                    flagBlocks[blockIndex] &= ~(1 << bitPosition);
                }
            }
        }

        public void SetAll()
        {
            for(int i = 0; i < flagBlocks.Length; i++)
            {
                flagBlocks[i] = 0xFFFF;
            }
        }

        public void ClearAll()
        {
            for (int i = 0; i < flagBlocks.Length; i++)
            {
                flagBlocks[i] = 0;
            }
        }

        public IEnumerator<bool> GetEnumerator()
        {
            for (int i = 0; i < flagBlocks.Length; i++)
            {
                int flagBlock = flagBlocks[i];
                yield return (flagBlock & 0x1) > 0;
                for (int _ = 0; _ < 31; _++)
                {
                    flagBlock >>= 1;
                    yield return (flagBlock & 0x1) > 0;
                }
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
