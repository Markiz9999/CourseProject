using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Курсовой_Проект
{
    class FileWithObjects
    {
        private List<Object3D> objects = new List<Object3D>();
        private List<int> objectsId = new List<int>();
        private List<Texture> textures = new List<Texture>();

        public List<Object3D> Objects { get { return objects; } }
        public List<int> ObjectsId { get { return objectsId; } }
        public List<Texture> Textures { get { return textures; } }

        public FileWithObjects(List<Object3D> Objects, List<int> ObjectsId, List<Texture> Textures) {
            objects = Objects;
            objectsId = ObjectsId;
            textures = Textures;
        }
    }
}
