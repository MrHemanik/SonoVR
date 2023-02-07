using mKit;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace SonoGame
{
    /// <summary>
    /// JSON-compatible wrapper class for the shape config list.
    /// </summary>
    [Serializable]
    public class ShapeConfigList : ISerializationCallbackReceiver
    {
        [SerializeField]
        public List<ShapeConfig> shapeConfigList;

        [SerializeField]
        protected List<ShapeConfigVoxel> shapeConfigListVoxel; // used as workaround for serialization limitation

        [SerializeField]
        protected List<ShapeConfig> shapeConfigListNorm;

        public ShapeConfigList(List<ShapeConfig> shapeConfigList)
        {
            this.shapeConfigList = shapeConfigList;
        }

        public ShapeConfigList(List<ShapeConfigVoxel> shapeConfigList)
        {
            this.shapeConfigList = new List<ShapeConfig>();
            foreach (var sc in shapeConfigList)
            {
                this.shapeConfigList.Add(sc);
            }
        }

        public void OnAfterDeserialize()
        {
            // restore subclass from separate list
            foreach (var shape in shapeConfigListVoxel)
            {
                shapeConfigList.Add(shape);
            }

            foreach (var shape in shapeConfigListNorm)
            {
                shapeConfigList.Add(shape);
            }

            shapeConfigListVoxel.Clear();
            shapeConfigListVoxel.Clear();
        }

        public void OnBeforeSerialize()
        {
            // store subclass in separate list
            shapeConfigListVoxel = new List<ShapeConfigVoxel>();
            shapeConfigListNorm = new List<ShapeConfig>();

            foreach (var shape in shapeConfigList)
            {
                if (shape is ShapeConfigVoxel voxelShapeConfig)
                {
                    shapeConfigListVoxel.Add(voxelShapeConfig);
                }
                else
                {
                    shapeConfigListNorm.Add(shape);
                }
            }
        }
    }

}
