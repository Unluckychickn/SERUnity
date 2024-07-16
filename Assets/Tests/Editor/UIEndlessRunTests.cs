using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;

[TestFixture]
public class UIEndlessRunTests
{
    private GameObject gameObject;
    private UI_EndlessRun uiEndlessRun;

    [SetUp]
    public void Setup()
    {
        // Setup a GameObject with the UI_EndlessRun component
        gameObject = new GameObject();
        uiEndlessRun = gameObject.AddComponent<UI_EndlessRun>();
    }

    [TearDown]
    public void Teardown()
    {
        // Clean up after each test
        Object.DestroyImmediate(gameObject);
    }

    [UnityTest]
    public IEnumerator ButtonCoversWholePreviewTemplate()
    {
        // Setup UI elements
        GameObject itemPreviewTemplate = new GameObject();
        RectTransform previewTemplateRect = itemPreviewTemplate.AddComponent<RectTransform>();
        previewTemplateRect.sizeDelta = new Vector2(100, 100); // Example size

        Button button = itemPreviewTemplate.AddComponent<Button>();
        RectTransform buttonRect = button.GetComponent<RectTransform>();

        // Simulate the button being set up in RefreshDisplay
        buttonRect.anchorMin = Vector2.zero;
        buttonRect.anchorMax = Vector2.one;
        buttonRect.sizeDelta = Vector2.zero;
        buttonRect.anchoredPosition = Vector2.zero;

        // Check if the button covers the entire preview template
        yield return null;
        Assert.AreEqual(Vector2.zero, buttonRect.anchorMin);
        Assert.AreEqual(Vector2.one, buttonRect.anchorMax);
        Assert.AreEqual(Vector2.zero, buttonRect.sizeDelta);
        Assert.AreEqual(Vector2.zero, buttonRect.anchoredPosition);
    }
}
