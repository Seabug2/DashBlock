using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public static class Locator
{
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
    private static void InitializeSingleton()
    {
        SceneManager.sceneUnloaded += (_) => ClearAllComponents();
    }

    readonly static Dictionary<Type, Singleton> dict_UIComponent = new();

    public static void RegisterComponent(Type _Type, Singleton _UIComponent)
    {
        if (!dict_UIComponent.ContainsKey(_Type))
        {
            dict_UIComponent.Add(_Type, _UIComponent);
        }
        else
        {
            Debug.Log("이미 추가되어있음");
            UnityEngine.Object.Destroy(_UIComponent.gameObject);
        }
    }

    public static void UnregisterComponent(Type _Type)
    {
        if (dict_UIComponent.ContainsKey(_Type))
        {
            dict_UIComponent.Remove(_Type);
        }
        else
        {
            Debug.LogWarning($"{_Type}에 해당하는 컴포넌트를 찾을 수 없습니다.");
        }
    }

    public static T GetUI<T>() where T : Singleton
    {
        Type type = typeof(T);
        if (dict_UIComponent.ContainsKey(type))
        {

            if (dict_UIComponent[type] is T component)
            {
                return component;
            }
            else
            {
                Debug.LogWarning($"{type}의 타입이 일치하지 않습니다.");
                return null;
            }

        }
        else
        {
            Debug.LogWarning($"{type}에 해당하는 컴포넌트가 없습니다.");
            return null;
        }
    }

    public static bool TryGetUI<T>(out T component) where T : Singleton
    {
        Type type = typeof(T);
        if (dict_UIComponent.TryGetValue(type, out Singleton uiComponent) && uiComponent is T castComponent)
        {
            component = castComponent;
            return true;
        }

        component = null;
        return false;
    }

    // 딕셔너리의 모든 UIComponent 제거
    public static void ClearAllComponents()
    {
        dict_UIComponent.Clear();
        Debug.Log("모든 UIComponent가 제거되었습니다.");
    }
}
