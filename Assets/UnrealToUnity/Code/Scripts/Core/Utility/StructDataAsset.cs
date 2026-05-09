using UnityEngine;

namespace UnrealToUnity.Code.Scripts.Core.Utility
{
    public class StructDataAsset<TStructType> : ScriptableObject
        where TStructType : struct
    {
        /// <summary>
        /// The struct data to be stored in this asset.
        /// </summary>
        [SerializeField] private TStructType data;

        public TStructType Data => data;

        /// <summary>
        /// A useful factory method for creating a scriptable object with the given data.
        /// </summary>
        /// <param name="data">The data to put in the data asset.</param>
        /// <returns>The scriptable object</returns>
        public static StructDataAsset<TStructType> CreateInstance(TStructType data)
        {
            var asset = CreateInstance<StructDataAsset<TStructType>>();
            asset.data = data;
            return asset;
        }
    }
}