namespace Fauna
{
    public abstract class Comparable
    {
        // Returns 1 if this object is greater than the specified object.
        // Returns -1 if this object is less than the specified object.
        // Returns 0 if this object is equal to the specified object.
        public abstract int CompareTo(object obj);
    }

    static class ArrayHelper
    {
        public static void Sort(object[] array)
        {
            for (int i = 0; i < array.Length; i++)
            {
                for (int j = i + 1; j < array.Length; j++)
                {
                    if (array[i] is not Comparable)
                    {
                        throw new ArgumentException("Object does not implement Comparable class.");
                    }
                    if ((array[i] as Comparable)!.CompareTo(array[j]) > 0)
                    {
                        object temp = array[i];
                        array[i] = array[j];
                        array[j] = temp;
                    }
                }
            }
        }
    }
}