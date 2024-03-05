namespace Epsiloner.Wpf.Collections
{
    ///<summary>
    ///
    ///</summary>
    ///<typeparam name="T">The type of elements in the collection.</typeparam>
    ///<param name="inserted">Indicates if item has been inserted or removed</param>
    ///<param name="item">Item which has been inserted or removed</param>
    /// <param name="index">Index of item in collection.</param>
    public delegate void ItemHandlerDelegate<T>(bool inserted, T item, int index);
}