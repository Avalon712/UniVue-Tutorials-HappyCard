
namespace HappyCard.Utils
{
    public abstract class Singleton<T> where T : new()
    {
        private static T _instanced;

        public static T Instance
        {
            get
            {
                if(_instanced == null) { _instanced = new T(); }

                return _instanced;
            }
        }
    }
}
