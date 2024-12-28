using Helpers;
using UnityEngine;

namespace Memories.Characters
{
    public sealed class CameraController : MonoSingleton<CameraController>
    {
        public Camera cam;
        public Transform target;
        public float pauseMenuXOffset = -1f;
        public float normalFov = 60f;
        public float pauseMenuFov = 30f;

        // using Vector2 for .SmoothDamp to match the tracking smoothing behaviour
        private Vector2 TargetFov => new(pauseMenuFov, 0);
        private Vector2 NormalFov => new(normalFov, 0);
        private static Vector2 _currentFovVelocity = Vector2.zero;

        [SerializeField]
        [Range(0.01f, 0.9f)]
        private float smoothTime = 0.35f;

        [Min(0)]
        public float maxDistance = 200f;

        internal static Vector3 currVelocity = Vector3.zero;
        internal static float DistanceToActivePlayer => Vector3.Distance(Characters.Player.ActivePlayer.transform.position, Camera.main!.transform.position);
        private static bool IsInPauseMenu => false; //PauseMenuBehaviour.IsOpen;
        private static bool UseScaledTime => !IsInPauseMenu;

        public void Update()
        {
            Vector2 fov = new(cam.fieldOfView, 0);
            float deltaTime = UseScaledTime ? Time.deltaTime : Time.unscaledDeltaTime;

            // cam.fieldOfView = Vector2.SmoothDamp(fov, IsInPauseMenu ? TargetFov : NormalFov, ref _currentFovVelocity, smoothTime, Mathf.Infinity, deltaTime).x;
            Vector3 desiredCamPos = target.position;
            // desiredCamPos.x = IsInPauseMenu ? desiredCamPos.x + pauseMenuXOffset : desiredCamPos.x;
            Vector3 camPos = transform.position;
            desiredCamPos.z = camPos.z;
            Vector3 direction = desiredCamPos - camPos;
            if (direction.magnitude > maxDistance)
                camPos = desiredCamPos - direction.normalized * maxDistance;
            camPos = Vector3.SmoothDamp(camPos, desiredCamPos, ref currVelocity, smoothTime, Mathf.Infinity, deltaTime);
            transform.position = camPos;
        }
    }
}