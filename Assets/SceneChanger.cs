using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SceneChanger : MonoBehaviour
{
    [SerializeField] private Button MyButton = null;
    // Start is called before the first frame update
    void Start()
    {
        MyButton.onClick.AddListener(() => {Application.LoadLevel(1);});
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
