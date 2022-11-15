using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class DisplayContextController : MonoBehaviour
{
    private static DisplayContextController _instance;
    public static DisplayContextController Instance
    {
        get {
            if (_instance == null) _instance = (DisplayContextController)FindObjectOfType(typeof(DisplayContextController));
            return _instance;
        }
    }
    
    private List<IDisplayContext> _displayContexts;
    private IDisplayContext _currentDisplayContext;

    private void Awake()
    {
        _displayContexts = FindObjectsOfType<MonoBehaviour>().OfType<IDisplayContext>().ToList();
    }

    public void HideAllAndDisplay(IDisplayContext newContext)
    {
        foreach (IDisplayContext displayContext in _displayContexts)
        {
            if (newContext != displayContext) displayContext.HideDisplayContext();
        }
        
        newContext.ShowDisplayContext();
        _currentDisplayContext = newContext;
    }

    public void HideCurrentAndDisplay(IDisplayContext newContext)
    {
        if (_currentDisplayContext != newContext) _currentDisplayContext?.HideDisplayContext();
        
        newContext.ShowDisplayContext();
        _currentDisplayContext = newContext;
    }

    public void HideAllAndToggle(IDisplayContext contextToToggle, bool toggle)
    {
        foreach (IDisplayContext displayContext in _displayContexts)
        {
            if (contextToToggle != displayContext) displayContext.HideDisplayContext();
        }

        if (toggle)
        {
            contextToToggle.ShowDisplayContext();
            _currentDisplayContext = contextToToggle;
        }
        else
        {
            contextToToggle.HideDisplayContext();
            _currentDisplayContext = null;
        }
    }
    
    public void HideCurrentAndToggle(IDisplayContext contextToToggle, bool toggle)
    {
        if (contextToToggle != _currentDisplayContext) _currentDisplayContext?.HideDisplayContext();
        
        if (toggle)
        {
            contextToToggle.ShowDisplayContext();
            _currentDisplayContext = contextToToggle;
        }
        else
        {
            contextToToggle.HideDisplayContext();
            _currentDisplayContext = null;
        }
    }

    public void HideAll()
    {
        foreach (IDisplayContext displayContext in _displayContexts)
        {
            displayContext.HideDisplayContext();
        }

        _currentDisplayContext = null;
    }

    public void HideCurrent()
    {
        if (_currentDisplayContext != null) _currentDisplayContext.HideDisplayContext();

        _currentDisplayContext = null;
    }
}