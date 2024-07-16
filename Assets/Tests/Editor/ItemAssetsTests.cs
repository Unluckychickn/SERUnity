using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.TestTools;
using UnityEditor.SceneManagement;

public class ItemAssetsTests
{
    private const string MenuScenePath = "Assets/Scenes/LevelSelect.unity";
    private const string ShopScenePath = "Assets/Scenes/Shop.unity";
    private const string GameScenePath = "Assets/Scenes/Endless Runner.unity";
    private const string GameOverScenePath = "Assets/Scenes/GameOver.unity";

    [SetUp]
    public void Setup()
    {
        // Load the initial scene in the editor
        EditorSceneManager.OpenScene(MenuScenePath);
    }

    private void LoadScene(string scenePath)
    {
        EditorSceneManager.OpenScene(scenePath);
    }

    [UnityTest]
    public IEnumerator SingletonInstanceTest()
    {
        // Wait for the initial scene to load
        yield return null;

        // Ensure there is only one instance of ItemAssets
        var instances = GameObject.FindObjectsOfType<ItemAssets>();
        Assert.AreEqual(1, instances.Length);

        // Transition to Shop scene and verify singleton
        LoadScene(ShopScenePath);
        yield return null;
        instances = GameObject.FindObjectsOfType<ItemAssets>();
        Assert.AreEqual(1, instances.Length);

        // Transition to Game scene and verify singleton
        LoadScene(GameScenePath);
        yield return null;
        instances = GameObject.FindObjectsOfType<ItemAssets>();
        Assert.AreEqual(1, instances.Length);

        // Transition to GameOver scene and verify singleton
        LoadScene(GameOverScenePath);
        yield return null;
        instances = GameObject.FindObjectsOfType<ItemAssets>();
        Assert.AreEqual(1, instances.Length);

        // Transition back to MenuScene and verify singleton
        LoadScene(MenuScenePath);
        yield return null;
        instances = GameObject.FindObjectsOfType<ItemAssets>();
        Assert.AreEqual(1, instances.Length);
    }

    [UnityTest]
    public IEnumerator MusicTransitionTest()
    {
        // Wait for the initial scene to load
        yield return null;
        Assert.IsNotNull(GameObject.FindObjectOfType<AudioSource>());

        // Transition to Shop scene and verify music transition
        LoadScene(ShopScenePath);
        yield return null;
        Assert.IsNotNull(GameObject.FindObjectOfType<AudioSource>());

        // Transition to Game scene and verify music transition
        LoadScene(GameScenePath);
        yield return null;
        Assert.IsNotNull(GameObject.FindObjectOfType<AudioSource>());

        // Transition to GameOver scene and verify music transition
        LoadScene(GameOverScenePath);
        yield return null;
        Assert.IsNotNull(GameObject.FindObjectOfType<AudioSource>());

        // Transition back to MenuScene and verify music transition
        LoadScene(MenuScenePath);
        yield return null;
        Assert.IsNotNull(GameObject.FindObjectOfType<AudioSource>());
    }
}
