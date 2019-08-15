using UnityEngine;

namespace ARPeerToPeerSample.Game
{
    public class CubeController : MonoBehaviour
    {
        [SerializeField, Tooltip("velocity of cube")]
        private float _velocity = .01f;

        [SerializeField, Tooltip("min pos")]
        private float _minPos = 1f;

        [SerializeField, Tooltip("max pos")]
        private float _maxPos = -1f;

        private float _velocityModifier = 1;

        private void Update()
        {
            float newXPos = this.transform.localPosition.x + _velocity * _velocityModifier * Time.deltaTime;
            if (newXPos <= _minPos)
            {
                this.transform.localPosition = new Vector3(_minPos, this.transform.localPosition.y, this.transform.localPosition.z);
                _velocityModifier *= -1f;
            }
            else if (newXPos >= _maxPos)
            {
                this.transform.localPosition = new Vector3(_maxPos, this.transform.localPosition.y, this.transform.localPosition.z);
                _velocityModifier *= -1f;
            }

            //this.transform.localPosition = new Vector3(newXPos, this.transform.localPosition.y, this.transform.localPosition.z);
        }
    }
}