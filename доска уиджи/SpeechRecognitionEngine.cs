using UnityEngine;
using UnityEngine.Windows.Speech;
using System.Collections;
using System.Collections.Generic;

public class OuijaBoardController : MonoBehaviour
{
    public Transform pointer; // стрелка
    public Transform[] letterPoints; // метки букв A-Z (размести их в порядке: A, B, C, ..., Z)
    public string triggerPhrase = "где ты"; // голосовая команда
    public string ghostRoom = "BATHROOM"; // название комнаты призрака

    public float moveDuration = 0.4f;
    public float delayBetweenLetters = 0.6f;

    private KeywordRecognizer recognizer;
    private bool isSpelling = false;

    void Start()
    {
        recognizer = new KeywordRecognizer(new string[] { triggerPhrase }, ConfidenceLevel.Medium);
        recognizer.OnPhraseRecognized += OnPhraseRecognized;
        recognizer.Start();
    }

    private void OnPhraseRecognized(PhraseRecognizedEventArgs args)
    {
        if (args.text == triggerPhrase && !isSpelling)
        {
            StartCoroutine(SpellRoomName());
        }
    }

    private IEnumerator SpellRoomName()
    {
        isSpelling = true;

        foreach (char c in ghostRoom.ToUpper())
        {
            int index = c - 'A';
            if (index >= 0 && index < letterPoints.Length)
            {
                yield return StartCoroutine(MovePointerTo(letterPoints[index]));
                yield return new WaitForSeconds(delayBetweenLetters);
            }
        }

        isSpelling = false;
    }

    private IEnumerator MovePointerTo(Transform target)
    {
        Vector3 start = pointer.position;
        Vector3 end = target.position;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            pointer.position = Vector3.Lerp(start, end, elapsed / moveDuration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        pointer.position = end;
    }

    private void OnApplicationQuit()
    {
        if (recognizer != null && recognizer.IsRunning)
        {
            recognizer.OnPhraseRecognized -= OnPhraseRecognized;
            recognizer.Stop();
        }
    }
}
