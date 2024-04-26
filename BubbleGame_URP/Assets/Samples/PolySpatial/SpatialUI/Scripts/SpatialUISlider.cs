using UnityEngine;
using UnityEngine.UI;

namespace PolySpatial.Samples
{
    public class SpatialUISlider : SpatialUI
    {
        [SerializeField]
        MeshRenderer m_FillRenderer;

        float m_BoxColliderSizeX;

        public float myPercent = 0.5f;

        void Start()
        {
            m_BoxColliderSizeX = GetComponent<BoxCollider>().size.x;
        }

        public float GetPercent()
        {
            return 1f - myPercent;
        }

        public override void Press(Vector3 position)
        {
            base.Press(position);
            var localPosition = transform.InverseTransformPoint(position);
            var percentage = localPosition.x / m_BoxColliderSizeX + 0.5f;
            myPercent = Mathf.Clamp(percentage, 0.0f, 1.0f);
            m_FillRenderer.material.SetFloat("_Percentage", myPercent);
        }
    }
}
