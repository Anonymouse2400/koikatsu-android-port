using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace NodeCanvas.DialogueTrees.UI.Examples
{
	public class DialogueUGUI : MonoBehaviour
	{
		[Serializable]
		public class SubtitleDelays
		{
			public float characterDelay = 0.05f;

			public float sentenceDelay = 0.5f;

			public float commaDelay = 0.1f;

			public float finalDelay = 1.2f;
		}

		[Header("Input Options")]
		public bool skipOnInput;

		public bool waitForInput;

		[Header("Subtitles")]
		public RectTransform subtitlesGroup;

		public Text actorSpeech;

		public Text actorName;

		public Image actorPortrait;

		public RectTransform waitInputIndicator;

		public SubtitleDelays subtitleDelays = new SubtitleDelays();

		[Header("Multiple Choice")]
		public RectTransform optionsGroup;

		public Button optionButton;

		private Dictionary<Button, int> cachedButtons;

		private Vector2 originalSubsPosition;

		private bool isWaitingChoice;

		private AudioSource _localSource;

		private AudioSource localSource
		{
			get
			{
				return (!(_localSource != null)) ? (_localSource = base.gameObject.AddComponent<AudioSource>()) : _localSource;
			}
		}

		private void OnEnable()
		{
			DialogueTree.OnDialogueStarted += OnDialogueStarted;
			DialogueTree.OnDialoguePaused += OnDialoguePaused;
			DialogueTree.OnDialogueFinished += OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest += OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest += OnMultipleChoiceRequest;
		}

		private void OnDisable()
		{
			DialogueTree.OnDialogueStarted -= OnDialogueStarted;
			DialogueTree.OnDialoguePaused -= OnDialoguePaused;
			DialogueTree.OnDialogueFinished -= OnDialogueFinished;
			DialogueTree.OnSubtitlesRequest -= OnSubtitlesRequest;
			DialogueTree.OnMultipleChoiceRequest -= OnMultipleChoiceRequest;
		}

		private void Start()
		{
			subtitlesGroup.gameObject.SetActive(false);
			optionsGroup.gameObject.SetActive(false);
			optionButton.gameObject.SetActive(false);
			waitInputIndicator.gameObject.SetActive(false);
			originalSubsPosition = subtitlesGroup.transform.position;
		}

		private void OnDialogueStarted(DialogueTree dlg)
		{
		}

		private void OnDialoguePaused(DialogueTree dlg)
		{
			subtitlesGroup.gameObject.SetActive(false);
			optionsGroup.gameObject.SetActive(false);
		}

		private void OnDialogueFinished(DialogueTree dlg)
		{
			subtitlesGroup.gameObject.SetActive(false);
			optionsGroup.gameObject.SetActive(false);
			if (cachedButtons == null)
			{
				return;
			}
			foreach (Button key in cachedButtons.Keys)
			{
				if (key != null)
				{
					UnityEngine.Object.Destroy(key.gameObject);
				}
			}
			cachedButtons = null;
		}

		private void OnSubtitlesRequest(SubtitlesRequestInfo info)
		{
			StartCoroutine(Internal_OnSubtitlesRequestInfo(info));
		}

		private IEnumerator Internal_OnSubtitlesRequestInfo(SubtitlesRequestInfo info)
		{
			string text = info.statement.text;
			AudioClip audio = info.statement.audio;
			IDialogueActor actor = info.actor;
			subtitlesGroup.gameObject.SetActive(true);
			actorSpeech.text = string.Empty;
			actorName.text = actor.name;
			actorSpeech.color = actor.dialogueColor;
			actorPortrait.gameObject.SetActive(actor.portraitSprite != null);
			actorPortrait.sprite = actor.portraitSprite;
			if (audio != null)
			{
				AudioSource actorSource = ((!(actor.transform != null)) ? null : actor.transform.GetComponent<AudioSource>());
				AudioSource playSource = ((!(actorSource != null)) ? localSource : actorSource);
				playSource.clip = audio;
				playSource.Play();
				actorSpeech.text = text;
				float timer = 0f;
				while (timer < audio.length)
				{
					if (skipOnInput && Input.anyKeyDown)
					{
						playSource.Stop();
						break;
					}
					timer += Time.deltaTime;
					yield return null;
				}
			}
			if (audio == null)
			{
				string tempText = string.Empty;
				bool inputDown = false;
				if (skipOnInput)
				{
					StartCoroutine(CheckInput(delegate
					{
						inputDown = true;
					}));
				}
				for (int i = 0; i < text.Length; i++)
				{
					if (skipOnInput && inputDown)
					{
						actorSpeech.text = text;
						yield return null;
						break;
					}
					if (!subtitlesGroup.gameObject.activeSelf)
					{
						yield break;
					}
					tempText += text[i];
					yield return StartCoroutine(DelayPrint(subtitleDelays.characterDelay));
					char c = text[i];
					if (c == '.' || c == '!' || c == '?')
					{
						yield return StartCoroutine(DelayPrint(subtitleDelays.sentenceDelay));
					}
					if (c == ',')
					{
						yield return StartCoroutine(DelayPrint(subtitleDelays.commaDelay));
					}
					actorSpeech.text = tempText;
				}
				if (!waitForInput)
				{
					yield return StartCoroutine(DelayPrint(subtitleDelays.finalDelay));
				}
			}
			if (waitForInput)
			{
				waitInputIndicator.gameObject.SetActive(true);
				while (!Input.anyKeyDown)
				{
					yield return null;
				}
				waitInputIndicator.gameObject.SetActive(false);
			}
			yield return null;
			subtitlesGroup.gameObject.SetActive(false);
			info.Continue();
		}

		private IEnumerator CheckInput(Action Do)
		{
			while (!Input.anyKeyDown)
			{
				yield return null;
			}
			Do();
		}

		private IEnumerator DelayPrint(float time)
		{
			float timer = 0f;
			while (timer < time)
			{
				timer += Time.deltaTime;
				yield return null;
			}
		}

		private void OnMultipleChoiceRequest(MultipleChoiceRequestInfo info)
		{
			optionsGroup.gameObject.SetActive(true);
			float height = optionButton.GetComponent<RectTransform>().rect.height;
			optionsGroup.sizeDelta = new Vector2(optionsGroup.sizeDelta.x, (float)info.options.Values.Count * height + 20f);
			cachedButtons = new Dictionary<Button, int>();
			int num = 0;
			foreach (KeyValuePair<IStatement, int> option in info.options)
			{
				Button btn = UnityEngine.Object.Instantiate(optionButton);
				btn.gameObject.SetActive(true);
				btn.transform.SetParent(optionsGroup.transform, false);
				btn.transform.localPosition = (Vector2)optionButton.transform.localPosition - new Vector2(0f, height * (float)num);
				btn.GetComponentInChildren<Text>().text = option.Key.text;
				cachedButtons.Add(btn, option.Value);
				btn.onClick.AddListener(delegate
				{
					Finalize(info, cachedButtons[btn]);
				});
				num++;
			}
			if (info.showLastStatement)
			{
				subtitlesGroup.gameObject.SetActive(true);
				float y = optionsGroup.position.y + optionsGroup.sizeDelta.y + 1f;
				subtitlesGroup.position = new Vector2(subtitlesGroup.position.x, y);
			}
			if (info.availableTime > 0f)
			{
				StartCoroutine(CountDown(info));
			}
		}

		private IEnumerator CountDown(MultipleChoiceRequestInfo info)
		{
			isWaitingChoice = true;
			float timer = 0f;
			while (timer < info.availableTime)
			{
				if (!isWaitingChoice)
				{
					yield break;
				}
				timer += Time.deltaTime;
				SetMassAlpha(optionsGroup, Mathf.Lerp(1f, 0f, timer / info.availableTime));
				yield return null;
			}
			if (isWaitingChoice)
			{
				Finalize(info, info.options.Values.Last());
			}
		}

		private void Finalize(MultipleChoiceRequestInfo info, int index)
		{
			isWaitingChoice = false;
			SetMassAlpha(optionsGroup, 1f);
			optionsGroup.gameObject.SetActive(false);
			if (info.showLastStatement)
			{
				subtitlesGroup.gameObject.SetActive(false);
				subtitlesGroup.transform.position = originalSubsPosition;
			}
			foreach (Button key in cachedButtons.Keys)
			{
				UnityEngine.Object.Destroy(key.gameObject);
			}
			info.SelectOption(index);
		}

		private void SetMassAlpha(RectTransform root, float alpha)
		{
			CanvasRenderer[] componentsInChildren = root.GetComponentsInChildren<CanvasRenderer>();
			foreach (CanvasRenderer canvasRenderer in componentsInChildren)
			{
				canvasRenderer.SetAlpha(alpha);
			}
		}
	}
}
