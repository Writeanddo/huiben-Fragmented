using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.Serialization;
using VFX.Book;

namespace Memories.Book
{
    [AddComponentMenu("Memory/Book")]
    public class MemoryBook : MonoBehaviour
    {
        private static readonly int _openProp = Animator.StringToHash("open");
        private static readonly int _pageProp = Animator.StringToHash("page");

        private Animator _animator;

        private Vector3 _startPos;
        private Vector3 _startRot;

        public MainSceneScript mainSceneScript;

        public Transform offShelfPosition;

        public BookSpread[] pageSpreads;

        public GameObject fakeCover;
        public GameObject realCover;
        public GameObject realArmature;

        public BookMaterialDriver materialDriver;

        [HideInInspector]
        public float pageSeparation;

        public bool animatorIsOpen;
        [FormerlySerializedAs("page")] public int animatorPage;

        [NonSerialized]
        public bool Advancing = true;

        [SerializeField]
        private State state = State.OnShelf;

        private enum State
        {
            Busy = -1,
            OnShelf,
            Previewing,
            Opened
        }

        public GameObject normalContainer;
        public GameObject deleteButtonContainer;

        public Color coverLightColor = new Color32(255, 244, 214, 255);
        public float coverLightIntensity = 1;

        private bool _unlocked;

        private void OnMouseDown()
        {
            TakeOut().Forget();
        }

        private void Awake()
        {
            _animator = GetComponent<Animator>();

            _startPos = transform.position;
            _startRot = transform.eulerAngles;

            if (fakeCover && realCover && realArmature && state == State.OnShelf)
            {
                fakeCover.SetActive(true);
                realCover.SetActive(false);
                realArmature.SetActive(false);
            }

            materialDriver.SetDefaults(false);
        }

        private void Update()
        {
            _animator.SetBool(_openProp, animatorIsOpen);
            _animator.SetInteger(_pageProp, animatorPage);

            if (UnityEngine.Input.GetKeyDown(KeyCode.C) && state == State.Opened) Close().Forget();
        }

        public void TurnPages(int pages)
        {
            if (pages == 0) return;

            Advancing = pages > 0;
            animatorPage = Mathf.Clamp(animatorPage + pages, 0, 11);
        }

        public void Unlock()
        {
            CoUnlock().Forget();
        }

        private async UniTask CoUnlock()
        {
            if (_unlocked) return;

            State oldState = state;
            state = State.Busy;

            materialDriver.Unlock();
            await UniTask.Delay(1000);
            _unlocked = true;

            state = oldState; // should only ever be OnShelf
        }

        private async UniTask TakeOut()
        {
            if (!mainSceneScript || mainSceneScript.activeBook || mainSceneScript.busy || !_unlocked || state != State.OnShelf) return;
            mainSceneScript.activeBook = this;

            state = State.Busy;

            await transform.LerpTransform(offShelfPosition, 0.3f);
            transform.LerpTransform(mainSceneScript.bookPreviewPosition, 1f).Forget();

            await UniTask.Delay(700);
            mainSceneScript.TakeOutBook();
            await UniTask.Delay(250);
            if (fakeCover) fakeCover.SetActive(false);
            if (realCover) realCover.SetActive(true);
            if (realArmature) realArmature.SetActive(true);
            await UniTask.Delay(250);

            state = State.Previewing;
        }

        public void PutBackEvent() => PutBack().Forget();

        private async UniTask PutBack()
        {
            if (!mainSceneScript || mainSceneScript.activeBook != this || state != State.Previewing) return;

            if (!offShelfPosition) return;

            state = State.Busy;

            mainSceneScript.PutBackBook();

            transform.DOMove(offShelfPosition.position, 1);
            transform.DORotate(offShelfPosition.eulerAngles, 1);

            if (fakeCover) fakeCover.SetActive(true);
            if (realCover) realCover.SetActive(false);
            if (realArmature) realArmature.SetActive(false);

            await UniTask.Delay(1000);

            transform.DOMove(_startPos, 0.3f);
            transform.DORotate(_startRot, 0.3f);
            await UniTask.Delay(300);

            state = State.OnShelf;
            mainSceneScript.activeBook = null;
        }

        public void OpenEvent() => Open().Forget();

        private async UniTask Open()
        {
            if (state != State.Previewing) return;

            state = State.Busy;

            normalContainer.SetActive(false);
            
            Advancing = true;
            animatorIsOpen = true;

            if (mainSceneScript) mainSceneScript.OpenBook();

            await UniTask.Delay(2500);

            state = State.Opened;
        }

        private async UniTask Close()
        {
            if (state != State.Opened) return;

            state = State.Busy;

            Advancing = false;
            animatorIsOpen = false;
            await UniTask.Delay(500);

            if (mainSceneScript) mainSceneScript.CloseBook().Forget();

            await UniTask.Delay(2500 - 500);

            deleteButtonContainer.SetActive(true);

            state = State.Previewing;
        }

        public void DeleteEvent() => Delete().Forget();

        private async UniTask Delete()
        {
            if (state != State.Previewing) return;

            state = State.Busy;

            fakeCover.SetActive(true);
            realCover.SetActive(false);
            realArmature.SetActive(false);

            materialDriver.Forget(1f);
            await UniTask.Delay(1000);

            gameObject.SetActive(false);
            mainSceneScript.PutBackBook();
            mainSceneScript.activeBook = null;

            mainSceneScript.currentlyUnlocked--;
        }

        public BookSpread GetCurrentSpread() => pageSpreads[animatorPage];
    }
}
