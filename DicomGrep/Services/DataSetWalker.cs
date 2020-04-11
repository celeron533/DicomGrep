using Dicom;
using Dicom.IO.Buffer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DicomGrep.Services
{
    public class DatasetWalker : IDicomDatasetWalker
    {
        public bool OnBeginFragment(DicomFragmentSequence fragment)
        {
            return true;
        }

        public bool OnBeginSequence(DicomSequence sequence)
        {
            // check Tag match
            return true;
        }

        public bool OnBeginSequenceItem(DicomDataset dataset)
        {
            return true;
        }

        public void OnBeginWalk()
        { }

        public bool OnElement(DicomElement element)
        {
            // check Tag match and Value match
            return true;
        }

        public Task<bool> OnElementAsync(DicomElement element)
        {
            return Task.FromResult(this.OnElement(element));
        }

        public bool OnEndFragment()
        {
            return true;
        }

        public bool OnEndSequence()
        {
            return true;
        }

        public bool OnEndSequenceItem()
        {
            return true;
        }

        public void OnEndWalk()
        { }

        public bool OnFragmentItem(IByteBuffer item)
        {
            return true;
        }

        public Task<bool> OnFragmentItemAsync(IByteBuffer item)
        {
            return Task.FromResult(this.OnFragmentItem(item));
        }
    }
}
