using System.Collections.Generic;
using System.Linq;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;

namespace EDCCommands
{
    /// <summary>
    /// This class contains additional LINQ extension methods that allow you to more easily work with
    /// AutoCAD objects and LINQ.
    /// </summary>
    public static class LinqExtensions
    {
        /// <summary>
        /// Returns an <c>IEnumerable&lt;T&gt;</c> based on the specified enumerable,
        /// and using the specified transaction and open mode.
        /// </summary>
        /// <typeparam name="T">A type that derives from <c>DBObject</c>.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="tr">The current transaction.</param>
        /// <param name="openMode">The open mode.</param>
        /// <returns>an <c>IEnumerable&lt;T&gt;</c>, where <c>T</c> is a type that derives from <c>DBObject</c>.</returns>
        public static IEnumerable<T> OfType<T>(this IEnumerable<ObjectId> enumerable,
            Transaction tr,
            OpenMode openMode)
            where T : DBObject
        {
            RXClass rxClass = RXObject.GetClass(typeof(T));

            return
                from ObjectId objectId in enumerable
                where objectId.ObjectClass.IsDerivedFrom(rxClass)
                select objectId.OpenAs<T>(tr, openMode);
        }

        /// <summary>
        /// Returns an <c>IEnumerable&lt;T&gt;</c> based on the specified enumerable,
        /// and using the specified transaction.
        /// </summary>
        /// <typeparam name="T">A type that derives from <c>DBObject</c>.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <param name="tr">The current transaction.</param>
        /// <returns>an <c>IEnumerable&lt;T&gt;</c>, where <c>T</c> is a type that derives from <c>DBObject</c>.</returns>
        public static IEnumerable<T> OfType<T>(this IEnumerable<ObjectId> enumerable,
            Transaction tr)
            where T : DBObject
        {
            return enumerable.OfType<T>(tr, OpenMode.ForRead);
        }

        /// <summary>
        /// Returns an <c>IEnumerable&lt;T&gt;</c> based on the specified selection set,
        /// and using the specified transaction and open mode.
        /// </summary>
        /// <typeparam name="T">A type that derives from <c>DBObject</c>.</typeparam>
        /// <param name="selectionSet">The selection set.</param>
        /// <param name="tr">The current transaction.</param>
        /// <param name="openMode">The open mode.</param>
        /// <returns>an <c>IEnumerable&lt;T&gt;</c>, where <c>T</c> is a type that derives from <c>DBObject</c>.</returns>
        public static IEnumerable<T> OfType<T>(this SelectionSet selectionSet,
            Transaction tr,
            OpenMode openMode)
            where T : DBObject
        {
            return selectionSet.Cast<ObjectId>().OfType<T>(tr, openMode);
        }

        /// <summary>
        /// Returns an <c>IEnumerable&lt;T&gt;</c> based on the specified selection set,
        /// and using the specified transaction.
        /// </summary>
        /// <typeparam name="T">A type that derives from <c>DBObject</c>.</typeparam>
        /// <param name="selectionSet">The selection set.</param>
        /// <param name="tr">The current transaction.</param>
        /// <returns>an <c>IEnumerable&lt;T&gt;</c>, where <c>T</c> is a type that derives from <c>DBObject</c>.</returns>
        public static IEnumerable<T> OfType<T>(this SelectionSet selectionSet,
            Transaction tr)
            where T : DBObject
        {
            return selectionSet.Cast<ObjectId>().OfType<T>(tr, OpenMode.ForRead);
        }

        /// <summary>
        /// Upgrades the open mode of each object in the <b>IEnumerable</b>.
        /// </summary>
        /// <typeparam name="T">The type of object.</typeparam>
        /// <param name="enumerable">The enumerable.</param>
        /// <returns>A new enumerable with the same set of objects.</returns>
        public static IEnumerable<T> UpgradeOpen<T>(this IEnumerable<T> enumerable) where T : DBObject
        {
            return from obj in enumerable select obj.ForWrite();
        }

    }
}