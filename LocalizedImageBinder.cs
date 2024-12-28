#if UNITY_EDITOR
#define UNITY_EDITOR_MODE
#endif

using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Localization.Components;

#if UNITY_EDITOR_MODE
using UnityEditor;
using UnityEditor.Events;
#endif

namespace RimuruDev
{
    /// <summary>
    /// Компонент для автоматической привязки локализованных спрайтов к Image с помощью LocalizeSpriteEvent.
    /// </summary>
    [SelectionBase]
    [ExecuteAlways]
    [DisallowMultipleComponent]
    [RequireComponent(typeof(Image))]
    [RequireComponent(typeof(RectTransform))]
    [RequireComponent(typeof(LocalizeSpriteEvent))]
    [AddComponentMenu("0x_/Localization/" + nameof(LocalizedImageBinder))]
    public sealed class LocalizedImageBinder : MonoBehaviour
    {
        private void Awake() =>
            SetupBindings();

        private void Reset() =>
            SetupBindings();

        private void OnDestroy()
        {
#if UNITY_EDITOR_MODE
            var localizeSpriteEvent = GetComponent<LocalizeSpriteEvent>();
            if (localizeSpriteEvent != null)
            {
                RemovePersistentListener(localizeSpriteEvent);
            }
#endif
        }

        /// <summary>
        /// Настраивает связь между LocalizeSpriteEvent и Image.
        /// </summary>
        private void SetupBindings()
        {
            var localizeSpriteEvent = GetComponent<LocalizeSpriteEvent>();
            var image = GetComponent<Image>();

            if (localizeSpriteEvent == null || image == null)
                return;

#if UNITY_EDITOR_MODE
            if (!HasPersistentListener(localizeSpriteEvent))
            {
                AddPersistentListener(localizeSpriteEvent);
            }
#endif
            ClearSprite();

            void ClearSprite() =>
                image.sprite = null;
        }

#if UNITY_EDITOR_MODE
        /// <summary>
        /// Проверяет, добавлен ли обработчик обновления спрайта.
        /// </summary>
        private bool HasPersistentListener(LocalizeSpriteEvent localizeSpriteEvent)
        {
            for (var i = 0; i < localizeSpriteEvent.OnUpdateAsset.GetPersistentEventCount(); i++)
            {
                if (localizeSpriteEvent.OnUpdateAsset.GetPersistentTarget(i) == this &&
                    localizeSpriteEvent.OnUpdateAsset.GetPersistentMethodName(i) == nameof(UpdateImageSprite))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Добавляет обработчик обновления спрайта для Image.
        /// </summary>
        private void AddPersistentListener(LocalizeSpriteEvent localizeSpriteEvent)
        {
            UnityEventTools.AddPersistentListener(localizeSpriteEvent.OnUpdateAsset, UpdateImageSprite);
            EditorUtility.SetDirty(localizeSpriteEvent);
        }

        /// <summary>
        /// Удаляет обработчик обновления спрайта для Image.
        /// </summary>
        private void RemovePersistentListener(LocalizeSpriteEvent localizeSpriteEvent)
        {
            for (var i = localizeSpriteEvent.OnUpdateAsset.GetPersistentEventCount() - 1; i >= 0; i--)
            {
                if (localizeSpriteEvent.OnUpdateAsset.GetPersistentTarget(i) == this &&
                    localizeSpriteEvent.OnUpdateAsset.GetPersistentMethodName(i) == nameof(UpdateImageSprite))
                {
                    UnityEventTools.RemovePersistentListener(localizeSpriteEvent.OnUpdateAsset, i);
                }
            }

            EditorUtility.SetDirty(localizeSpriteEvent);
        }
#endif

        /// <summary>
        /// Обновляет спрайт компонента Image.
        /// </summary>
        /// <param name="sprite">Новый локализованный спрайт.</param>
        private void UpdateImageSprite(Sprite sprite)
        {
            var image = GetComponent<Image>();
            if (image != null)
            {
                image.sprite = sprite;
            }
        }
    }
}