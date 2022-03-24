using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExpressionController : MonoBehaviour
{
    Expression exp;
    public Sprite happySprite, sadSprite, deadSprite, defaultSprite;
    public Image expressionImage;
    public Expression.ExpressionType currentExpression;
    public float expressionTime = 1.0f;
    float expressionTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        exp = GetComponentInChildren<Expression>();
    }

    // Update is called once per frame
    void Update()
    {
        expressionTimer -= Time.deltaTime;
        if(expressionTimer <= 0.0f)
        {
            currentExpression = Expression.ExpressionType.Default;
        }
        switch (exp.currentExpression)
        {
            case Expression.ExpressionType.Default:
                expressionImage.sprite = defaultSprite;
                break;
            case Expression.ExpressionType.Happy:
                expressionImage.sprite = happySprite;
                break;
            case Expression.ExpressionType.Sad:
                expressionImage.sprite = sadSprite;
                break;
            case Expression.ExpressionType.Dead:
                expressionImage.sprite = deadSprite;
                break;
        }
        exp.currentExpression = currentExpression;
    }

    public void SetExpression(Expression.ExpressionType expression)
    {
        currentExpression = expression;
        expressionTimer = expressionTime;
    }
}
