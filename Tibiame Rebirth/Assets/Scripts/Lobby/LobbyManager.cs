using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

namespace Lobby
{
    public class LobbyManager : MonoBehaviour
    {
        [Header("Button References")]
        public Button playButton;
        public Button settingsButton;
        public Button exitButton;
        
        [Header("Animation Settings")]
        public float hoverScale = 1.1f;
        public float hoverDuration = 0.2f;
        public float clickScale = 0.95f;
        public float clickDuration = 0.1f;
        
        [Header("Audio Settings")]
        public AudioClip hoverSound;
        public AudioClip clickSound;
        public AudioSource audioSource;
        
        [Header("Visual Effects")]
        public GameObject buttonGlowEffect;
        public Color playButtonColor = Color.green;
        public Color settingsButtonColor = Color.blue;
        public Color exitButtonColor = Color.red;

        private void Start()
        {
            InitializeButtons();
            StartIdleAnimations();
        }

        private void InitializeButtons()
        {
            // Play Button
            if (playButton != null)
            {
                playButton.onClick.AddListener(OnPlayButtonClicked);
                AddButtonEffects(playButton, playButtonColor);
            }

            // Settings Button
            if (settingsButton != null)
            {
                settingsButton.onClick.AddListener(OnSettingsButtonClicked);
                AddButtonEffects(settingsButton, settingsButtonColor);
            }

            // Exit Button
            if (exitButton != null)
            {
                exitButton.onClick.AddListener(OnExitButtonClicked);
                AddButtonEffects(exitButton, exitButtonColor);
            }
        }

        private void AddButtonEffects(Button button, Color buttonColor)
        {
            // Add hover and click effects
            var eventTrigger = button.gameObject.AddComponent<UnityEngine.EventSystems.EventTrigger>();
            
            // Hover Enter
            var hoverEntry = new UnityEngine.EventSystems.EventTrigger.Entry();
            hoverEntry.eventID = UnityEngine.EventSystems.EventTriggerType.PointerEnter;
            hoverEntry.callback.AddListener((data) => OnButtonHoverEnter(button, buttonColor));
            eventTrigger.triggers.Add(hoverEntry);

            // Hover Exit
            var hoverExit = new UnityEngine.EventSystems.EventTrigger.Entry();
            hoverExit.eventID = UnityEngine.EventSystems.EventTriggerType.PointerExit;
            hoverExit.callback.AddListener((data) => OnButtonHoverExit(button));
            eventTrigger.triggers.Add(hoverExit);

            // Pointer Down
            var pointerDown = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerDown.eventID = UnityEngine.EventSystems.EventTriggerType.PointerDown;
            pointerDown.callback.AddListener((data) => OnButtonPointerDown(button));
            eventTrigger.triggers.Add(pointerDown);

            // Pointer Up
            var pointerUp = new UnityEngine.EventSystems.EventTrigger.Entry();
            pointerUp.eventID = UnityEngine.EventSystems.EventTriggerType.PointerUp;
            pointerUp.callback.AddListener((data) => OnButtonPointerUp(button));
            eventTrigger.triggers.Add(pointerUp);
        }

        private void OnButtonHoverEnter(Button button, Color color)
        {
            PlayHoverSound();
            StartCoroutine(ScaleButton(button, hoverScale, hoverDuration));
            
            // Add glow effect
            if (buttonGlowEffect != null)
            {
                GameObject glow = Instantiate(buttonGlowEffect, button.transform);
                glow.transform.localPosition = Vector3.zero;
                glow.transform.localScale = Vector3.one;
                
                // Change glow color based on button
                var glowRenderer = glow.GetComponent<SpriteRenderer>();
                if (glowRenderer != null)
                {
                    glowRenderer.color = color;
                }
                
                Destroy(glow, hoverDuration);
            }
        }

        private void OnButtonHoverExit(Button button)
        {
            StartCoroutine(ScaleButton(button, 1f, hoverDuration));
        }

        private void OnButtonPointerDown(Button button)
        {
            PlayClickSound();
            StartCoroutine(ScaleButton(button, clickScale, clickDuration));
        }

        private void OnButtonPointerUp(Button button)
        {
            StartCoroutine(ScaleButton(button, 1f, clickDuration));
        }

        private IEnumerator ScaleButton(Button button, float targetScale, float duration)
        {
            Vector3 startScale = button.transform.localScale;
            Vector3 targetScaleVector = new Vector3(targetScale, targetScale, targetScale);
            float elapsedTime = 0f;

            while (elapsedTime < duration)
            {
                button.transform.localScale = Vector3.Lerp(startScale, targetScaleVector, elapsedTime / duration);
                elapsedTime += Time.deltaTime;
                yield return null;
            }

            button.transform.localScale = targetScaleVector;
        }

        private void StartIdleAnimations()
        {
            StartCoroutine(IdleAnimationRoutine());
        }

        private IEnumerator IdleAnimationRoutine()
        {
            while (true)
            {
                // Gentle floating animation for all buttons
                if (playButton != null)
                {
                    yield return StartCoroutine(FloatButton(playButton));
                }
                if (settingsButton != null)
                {
                    yield return StartCoroutine(FloatButton(settingsButton));
                }
                if (exitButton != null)
                {
                    yield return StartCoroutine(FloatButton(exitButton));
                }
                
                yield return new WaitForSeconds(2f);
            }
        }

        private IEnumerator FloatButton(Button button)
        {
            Vector3 originalPosition = button.transform.localPosition;
            float floatHeight = 5f;
            float floatDuration = 1f;
            
            // Float up
            float elapsedTime = 0f;
            while (elapsedTime < floatDuration / 2f)
            {
                float newY = Mathf.Lerp(originalPosition.y, originalPosition.y + floatHeight, elapsedTime / (floatDuration / 2f));
                button.transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            // Float down
            elapsedTime = 0f;
            while (elapsedTime < floatDuration / 2f)
            {
                float newY = Mathf.Lerp(originalPosition.y + floatHeight, originalPosition.y, elapsedTime / (floatDuration / 2f));
                button.transform.localPosition = new Vector3(originalPosition.x, newY, originalPosition.z);
                elapsedTime += Time.deltaTime;
                yield return null;
            }
            
            button.transform.localPosition = originalPosition;
        }

        private void PlayHoverSound()
        {
            if (audioSource != null && hoverSound != null)
            {
                audioSource.PlayOneShot(hoverSound);
            }
        }

        private void PlayClickSound()
        {
            if (audioSource != null && clickSound != null)
            {
                audioSource.PlayOneShot(clickSound);
            }
        }

        public void OnPlayButtonClicked()
        {
            Debug.Log("Play button clicked!");
            // Load game scene
            SceneManager.LoadScene("World1");
        }

        public void OnSettingsButtonClicked()
        {
            Debug.Log("Settings button clicked!");
            // Open settings panel (you can implement this later)
            // For now, we'll just log it
        }

        public void OnExitButtonClicked()
        {
            Debug.Log("Exit button clicked!");
            
            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }
    }
}
