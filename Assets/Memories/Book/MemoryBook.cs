using System.Linq;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using Memories.Characters;
using Memories.Cutscenes;
using UnityEngine;
using UnityEngine.Serialization;

namespace Memories.Book
{
    [AddComponentMenu("Memory/Book")]
    public class MemoryBook : MonoBehaviour
    {
        private static readonly int _openProp = Animator.StringToHash("open");

        private Animator _animator;

        private Vector3 _startPos;

        private Vector3 _cameraStartPos;
        private Vector3 _cameraStartRot;

        public Transform offShelfPosition;
        [FormerlySerializedAs("readingPosition")] public Transform previewPosition;

        public Transform cameraTransform;
        public Transform cameraPreviewLocation;
        public Transform cameraReadingLocation;
        public GameObject bookshelfObject;

        public BookActor[] actors;
        public CustomSequencer[] customSequences;

        [HideInInspector]
        public bool inTransition;

        [HideInInspector]
        public float pageSeparation;

        public bool open;

        public float popupProgress = 0;

        private Popup[] _popups;

        private void Awake()
        {
            _popups = GetComponentsInChildren<Popup>();

            _animator = GetComponent<Animator>();
            _startPos = transform.position;

            _cameraStartPos = cameraTransform != null ? cameraTransform.position : Vector3.zero;
            _cameraStartRot = cameraTransform != null ? cameraTransform.eulerAngles : Vector3.zero;
        }

        private void Update()
        {
            _animator.SetBool(_openProp, open);

            foreach (Popup popup in _popups)
            {
                popup.DoRotate(popupProgress);
            }

            // float absDiff = Mathf.Abs(left.transform.localEulerAngles.y - right.transform.localEulerAngles.y);
            // float degreeDiff = absDiff % 360;
            // const float oneOver180 = 1f / 180f; // multiplication is faster than division;
            // pageSeparation = degreeDiff * oneOver180;

            if (UnityEngine.Input.GetKeyDown(KeyCode.T)) TakeOut().Forget();
            if (UnityEngine.Input.GetKeyDown(KeyCode.O)) Open().Forget();
            if (UnityEngine.Input.GetKeyDown(KeyCode.C)) Close().Forget();
            if (UnityEngine.Input.GetKeyDown(KeyCode.P)) Stuff().Forget();
        }

        public async UniTask Stuff()
        {
            while (true)
            {
                await Open();
                await UniTask.Delay(2000);
                await Close();
                await UniTask.Delay(2000);
            }
        }

        public async UniTask TakeOut()
        {
            inTransition = true;
            await transform.LerpTransform(offShelfPosition, 0.3f);
            transform.LerpTransform(previewPosition, 1f).Forget();

            await UniTask.Delay(700);
            cameraTransform.DOMove(cameraPreviewLocation.position, 0.5f);
            cameraTransform.DORotate(cameraPreviewLocation.eulerAngles, 0.5f);
            inTransition = false;
        }

        private async UniTask Open()
        {
            bookshelfObject.SetActive(false);
            open = true;
            cameraTransform.DOMove(cameraReadingLocation.position, 0.85f);
            cameraTransform.DORotate(cameraReadingLocation.eulerAngles, 0.85f);
        }

        private async UniTask Close()
        {
            open = false;
            await UniTask.Delay(500);
            cameraTransform.DOMove(cameraPreviewLocation.position, 0.7f);
            cameraTransform.DORotate(cameraPreviewLocation.eulerAngles, 0.7f);
            await UniTask.Delay(700);
            bookshelfObject.SetActive(true);
        }

        public CustomSequencer GetSequencer(string sequenceName)
        {
            return customSequences.FirstOrDefault(s => s.sequenceName == sequenceName);
        }
    }
}
