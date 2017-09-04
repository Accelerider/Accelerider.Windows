using System.Collections.Generic;
using System.Collections.ObjectModel;
using Accelerider.Windows.Infrastructure.Interfaces;

namespace Accelerider.Windows.ViewModels.Others
{
    public class TransferedTaskList : ObservableCollection<ITransferedFile>
    {
        public TransferedTaskList(IEnumerable<ITransferedFile> files)
        {
            foreach (var file in files)
            {
                Add(file);
            }
        }

        public void Add(ITransferTaskToken token) 
        {
            InsertItem(0, token.GetTransferedFile());
        }

        protected override void InsertItem(int index, ITransferedFile item)
        {
            base.InsertItem(GetAppropriateIndex(item), item);
        }


        private void SubscribeCheckedEvent(ITransferedFile file)
        {
            
        }

        private void OnChecked()
        {
            
        }

        private int GetAppropriateIndex(ITransferedFile other)
        {
            int i = 0;
            for (; i < Items.Count; i++)
            {
                var item = Items[i];
                if (other.CompletedTime < item.CompletedTime) break;
            }
            return i;
        }
    }
}
