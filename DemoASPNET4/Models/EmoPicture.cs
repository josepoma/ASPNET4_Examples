using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DemoASPNET4.Models
{
    public class EmoPicture
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Path { get; set; }

        //Property Navigation
        public virtual ObservableCollection<EmoFace> Faces { get; set; }

    }
}
