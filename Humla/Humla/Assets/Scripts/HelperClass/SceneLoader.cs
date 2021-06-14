using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum LoadingStatus
{
    NONE,
    LOADING,
    FINISHED
};

public delegate void OnSceneLoadedDelegate(Scene scene);
public delegate void OnSceneUpdatedDelegate(float progress);

public sealed class LoadingRequest
{
    public string Path { get; private set; }
    public LoadingStatus Status { get; set; }
    public bool LoadAsynchronously { get; private set; }
    public OnSceneLoadedDelegate OnSceneLoaded { get; private set; }
    public OnSceneUpdatedDelegate OnSceneUpdated { get; private set; }

    public LoadingRequest(string path, bool loadAsync, OnSceneUpdatedDelegate onSceneUpdated,
                    OnSceneLoadedDelegate onSceneLoaded)
    {
        Path = path;
        LoadAsynchronously = loadAsync;
        OnSceneLoaded = onSceneLoaded;
        OnSceneUpdated = onSceneUpdated;
        Status = LoadingStatus.NONE;
    }
};

public class SceneLoader : Singleton<SceneLoader>
{
    private List<LoadingRequest> _loadingRequestQueue;
    private LoadingRequest _currentRequest;
    public AsyncOperation LoadOpeartion { get; private set; }
    public SceneLoader()
    {
        _currentRequest = null;
        _loadingRequestQueue = new List<LoadingRequest>();
    }

    public Scene LoadImmediately(string path)
    {
        LoadSceneParameters loadSceneParameters = new LoadSceneParameters(LoadSceneMode.Single);
        return SceneManager.LoadScene(path, loadSceneParameters);
    }

    public void AddRequest(string path, bool loadAsync, OnSceneUpdatedDelegate onSceneUpdated,
                    OnSceneLoadedDelegate onSceneLoaded)
    {
        LoadingRequest request = new LoadingRequest(path, loadAsync, onSceneUpdated, onSceneLoaded);
        _loadingRequestQueue.Add(request);
    }

    public void Update(double deltaTime)
    {
        if (_currentRequest == null && _loadingRequestQueue.Count > 0)
        {
            _currentRequest = _loadingRequestQueue[0];
        }

        if (_currentRequest != null)
        {
            switch (_currentRequest.Status)
            {
                case LoadingStatus.NONE:
                    if (!_currentRequest.LoadAsynchronously)
                    {
                        LoadImmediately(_currentRequest.Path);
                        _currentRequest.Status = LoadingStatus.FINISHED;
                    }
                    else
                    {
                        MainInstance.Instance.StartCoroutine(LoadSceneAsync());
                        _currentRequest.Status = LoadingStatus.LOADING;
                    }
                    break;
                case LoadingStatus.LOADING:
                    if (LoadOpeartion != null)
                    {
                        _currentRequest.OnSceneUpdated?.Invoke(LoadOpeartion.progress);
                    }
                    break;
                case LoadingStatus.FINISHED:
                    _currentRequest.OnSceneLoaded?.Invoke(SceneManager.GetActiveScene());
                    _loadingRequestQueue.Remove(_currentRequest);
                    _currentRequest = null;
                    break;
            }
        }
    }

    private IEnumerator LoadSceneAsync()
    {
        LoadOpeartion = SceneManager.LoadSceneAsync(_currentRequest.Path, LoadSceneMode.Single);
        LoadOpeartion.allowSceneActivation = false;
        while(LoadOpeartion.progress < 0.89)//Stupid Unity
        {                
            yield return null;
        }

        LoadOpeartion.allowSceneActivation = true;
        _currentRequest.Status = LoadingStatus.FINISHED;
    }
}

