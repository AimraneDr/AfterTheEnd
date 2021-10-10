using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class simpleMenuUIController : MonoBehaviour
{
    public Button button;
    public GameObject scrollView;

    bool scroll_view_is_visible;

    // Start is called before the first frame update
    void Start()
    {
        scroll_view_is_visible = false;
        button.onClick.AddListener(OnButtonClick);
    }

    // Update is called once per frame
    void Update()
    {
       
    }

    public void OnButtonClick()
    {
        Debug.Log("Event fired up");
        scrollView.SetActive(!scroll_view_is_visible);
        scroll_view_is_visible = !scroll_view_is_visible;
    }
}
