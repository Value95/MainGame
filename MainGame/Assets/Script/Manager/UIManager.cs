using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Serialization;

public class UIManager : BaseManager<UIManager>
{
      [SerializeField] private GameObject windowParent;
        [SerializeField] private GameObject popupParent;

        // 현재 까지 열린 ui
        private Dictionary<string, BaseUI> _windowPool = new Dictionary<string, BaseUI>();
        private Dictionary<string, BaseUI> _popupPool = new Dictionary<string, BaseUI>();
        private Dictionary<string, BaseUI> _toastMessagePool = new Dictionary<string, BaseUI>();

        // 현재 열려있는 ui
        private Queue<BaseUI> _windowOpenHistory;
        public Queue<BaseUI> WindowOpenHistory => _windowOpenHistory;
        private Queue<BaseUI> _popupOpenHistory;
        public Queue<BaseUI> PopupOpenHistory => _popupOpenHistory;
        private BaseUI toastMessage;

        private Stack<BaseUI> _windowCloseHistory;

        public override void Prepare()
        {
        }

        public override void Run()
        {
            _windowOpenHistory = new Queue<BaseUI>();
            _popupOpenHistory = new Queue<BaseUI>();
            _windowCloseHistory = new Stack<BaseUI>();
        }

        public T GetWindow<T>() where T : BaseUI
        {
            foreach (var window in _windowOpenHistory)
            {
                var w = window as T;
                if (w != null)
                    return w;
            }

            return null;
        }

        #region Window

        public BaseUI ShowWindow<T>(params object[] eParam) where T : BaseUI, new()
        {
            var window = _ShowWindow<T>(eParam);

            return window as T;
        }

        private async UniTask<BaseUI> _ShowWindow<T>(params object[] eParam) where T : BaseUI, new()
        {
            await _OpenWindowClose();

            var path = new T().Path;
            T baseUI;

            if (_windowPool.TryGetValue(path, out var window))
            {
                baseUI = window as T;
            }
            else
            {
                GameObject prefab = Resources.Load<GameObject>(path);
                var instanceWindow = GameObject.Instantiate(prefab, windowParent.transform);
                baseUI = instanceWindow.GetComponent<T>();
                _windowPool.Add(path, baseUI);
            }

            if (!baseUI)
                return null;

            _BaseUiOpen<T>(baseUI, eParam);

            _windowOpenHistory.Enqueue(baseUI);
            return baseUI;
        }
        
        public void CloseWindow()
        {
            if (!_windowOpenHistory.TryDequeue(out var window))
            {
                return;
            }

            if (!window)
            {
                DebugEx.Log("window is Null");
                return;
            }

            window.Close();
            window.AnimTrigger("Out", () =>
            {
                window.CloseAnim();
                window.gameObject.SafeSetActive(false);
                _CloseWindowOpen();
            }).Forget();
        }

        // 윈도우를 열때 열려있는 윈도우가있다면 닫는다.
        private Task _OpenWindowClose()
        {
            if (!_windowOpenHistory.TryDequeue(out var window))
            {
                return Task.CompletedTask;
            }

            _windowCloseHistory.Push(window);
            window.AnimTrigger("Out", () => { window.gameObject.SafeSetActive(false); }).Forget();
            return Task.CompletedTask;
        }

        // 윈도우를 닫을때 이전에 열려있던 윈도우가 있다면 열어준다.
        private void _CloseWindowOpen()
        {
            if (_windowCloseHistory.Count >= 1)
                return;

            if (!_windowCloseHistory.TryPop(out var window))
            {
                return;
            }

            window.AnimTrigger("In", () => { window.gameObject.SafeSetActive(true); }).Forget();
        }
        
        #endregion

        #region Popup

        public BaseUI ShowPopup<T>(params object[] eParam) where T : BaseUI, new()
        {
            var path = new T().Path;
            T baseUI;
            if (_popupPool.TryGetValue(path, out var popup))
            {
                baseUI = popup as T;
            }
            else
            {
                GameObject windowPrefab = Resources.Load<GameObject>(path);
                var instancePopup = GameObject.Instantiate(windowPrefab, popupParent.transform);
                baseUI = instancePopup.GetComponent<T>();
                _popupPool.Add(path, baseUI);
            }

            if (baseUI == null)
                return null;

            _BaseUiOpen<T>(baseUI, eParam);

            _popupOpenHistory.Enqueue(baseUI);
            return baseUI;
        }

        public void ShowYesNoPopup(string pTitle, string pDocument,
            Action pYesCallBack = null, Action pNoCallBack = null)
        {
            var popup = ShowPopup<DefaultPopup>();
            (popup as DefaultPopup)?.ShowYesNo(pTitle, pDocument, pYesCallBack, pNoCallBack);
        }

        public void ShowYsePopup(string pTitle, string pDocument,
            Action pYesCallBack = null)
        {
            var popup = ShowPopup<DefaultPopup>();
            (popup as DefaultPopup)?.ShowYes(pTitle, pDocument, pYesCallBack);
        }

        public void ShowCancelPopup(string pTitle, string pDocument,
            Action pCancelCallBack = null)
        {
            var popup = ShowPopup<DefaultPopup>();
            (popup as DefaultPopup)?.ShowCancel(pTitle, pDocument, pCancelCallBack);
        }
        
        public void ClosePopup()
        {
            if (!_popupOpenHistory.TryDequeue(out var popup))
            {
                return;
            }

            if (popup == null)
            {
                DebugEx.Log("popup is Null");
                return;
            }

            popup.Close();
            popup.AnimTrigger("Out", () =>
            {
                popup.CloseAnim();
                popup.gameObject.SafeSetActive(false);
            }).Forget();
        }

        #endregion

        #region ToastMessage

        public BaseUI ShowToastMessage<T>(string pMessage)where T : BaseUI, new()
        {
            var path = new T().Path;
            T baseUI;
            if (_toastMessagePool.TryGetValue(path, out var popup))
            {
                baseUI = popup as T;
            }
            else
            {
                GameObject windowPrefab = Resources.Load<GameObject>(path);
                var instancePopup = GameObject.Instantiate(windowPrefab, popupParent.transform);
                baseUI = instancePopup.GetComponent<T>();
                _popupPool.Add(path, baseUI);
            }

            if (baseUI == null)
                return null;

            _BaseUiOpen<T>(baseUI, new []{pMessage});

            toastMessage = baseUI;
            return baseUI;   
        }

        public void CloseToastMessage()
        {
            toastMessage.Close();
            toastMessage.AnimTrigger("Out", () =>
            {
                toastMessage.CloseAnim();
                toastMessage.gameObject.SafeSetActive(false);
            }).Forget();
        }
        
        #endregion
      

        private void _BaseUiOpen<T>(T eUI, object[] eParam) where T : BaseUI, new()
        {
            if (!eUI)
            {
                DebugEx.Log("eUI is Null");
                return;
            }

            eUI.gameObject.SetActive(true);
            eUI.AnimTrigger("In", () => { eUI.Open(eParam); }).Forget();
        }
}
