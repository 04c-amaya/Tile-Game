using System.Collections;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
using UnityEngine.VFX;

public class Movement : MonoBehaviour
{
    enum Direction
    {
        Right,
        Left,
        Up,
        Down,
        None
    }
    Direction swipeDirection;
    VisualElement root;
    List<VisualElement> pieces;
    VisualElement activeElement;
    
     float deltaX;
     float deltaY;

    Vector2 downPosition;
    Vector2 upPosition;
    Vector3 moveDirection;

    float unitSize = 100;
    public float absoluteX;
    public float absoluteY;

    public bool gameIsWon;

    VisualElement sensor;
    bool tileBlocked;
    Label wonMessage;

    private void Awake()
    {
        root= GetComponent<UIDocument>().rootVisualElement.Query<VisualElement>();
        pieces = root.Query<VisualElement>().ToList();
        wonMessage = root.Q<Label>();

    }
    private void Start()
    {
        root.RegisterCallback<PointerDownEvent>(DownPosition);
        root.RegisterCallback<PointerUpEvent>(UpPosition);
        wonMessage.style.display = DisplayStyle.None;
    }
    private void Update()
    {

    }

    void SwipeAnalysis()
    {
        deltaX = upPosition.x - downPosition.x;
        deltaY = upPosition.y - downPosition.y;

        absoluteX = Mathf.Abs(deltaX);
        absoluteY = Mathf.Abs(deltaY);

        if(absoluteX > absoluteY && activeElement.name.Contains("H"))
        {
            if (deltaX > 0)
            {
                swipeDirection = Direction.Right;
            }
            else
            {
                swipeDirection = Direction.Left;
            }
        }
        else if (absoluteX < absoluteY && activeElement.name.Contains("V"))
        {
            if (deltaY > 0)
            {
                swipeDirection = Direction.Down;
            }
            else
            {
                swipeDirection = Direction.Up;
            }
        }
        else
        {
            swipeDirection=Direction.None;
        }
    if(swipeDirection != Direction.None)
        {
            MoveTile();
        }
    }
    void DownPosition(PointerDownEvent eventInfo)
    {
        downPosition = eventInfo.position;
        activeElement = eventInfo.target as VisualElement;

    }
    void UpPosition(PointerUpEvent eventInfo)
    {
        upPosition = eventInfo.position;
        if (activeElement.name.Contains("Piece"))
        {
            SwipeAnalysis();
        }
        else
        {
            Debug.Log("Tile not selected");
        }
    }
    void MoveTile()
    {
        switch (swipeDirection)
        {
            case Direction.Right:
                 moveDirection = new Vector3(unitSize, 0, 0);
                sensor = activeElement.Q<VisualElement>("Sensor_R");
                break;
            case Direction.Left:
                moveDirection = new Vector3(-unitSize, 0, 0);
                sensor = activeElement.Q<VisualElement>("Sensor_L");
                break;
            case Direction.Up:
                moveDirection = new Vector3(0, -unitSize, 0);
                sensor = activeElement.Q<VisualElement>("Sensor_T");
                break;
            case Direction.Down:
                moveDirection = new Vector3(0, unitSize, 0);
                sensor = activeElement.Q<VisualElement>("Sensor_B");
                break;
        }
        tileBlocked = false;
        foreach(VisualElement item in pieces)
        {
            if((item.name.Contains("Piece")&& item != activeElement)|| item.name.Contains("Border"))
            {
                if (sensor.worldBound.Overlaps(item.worldBound))
                {
                    tileBlocked = true;
                }
            }
            else if (item.name.Contains("Goal"))
            {
                if (sensor.worldBound.Overlaps(item.worldBound))
                {
                    gameIsWon = true;
                }
            }
        }
        if (tileBlocked)
        {
            Debug.Log("Tile Blocked");
        }
        else
        {
            activeElement.transform.position += moveDirection;
            if (gameIsWon)
            {
                Debug.Log("Game is Won");
                activeElement.style.display = DisplayStyle.None;
                wonMessage.style.display = DisplayStyle.Flex;
                Invoke(nameof(GameWon), 2f);
            }
        }

    }
    void GameWon()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
