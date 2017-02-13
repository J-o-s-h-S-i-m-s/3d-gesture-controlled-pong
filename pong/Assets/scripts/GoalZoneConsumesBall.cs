using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GoalZoneConsumesBall : MonoBehaviour
{
    public GameObject gameOver;

    private float thisGoalZoneX;

    private float thisGoalZoneZ;

    private const float goalZone01X = 10;

    private const float goalZone01LeftZ = -(19 / 6);

    private const float goalZone01MiddleZ = 0;

    private const float goalZone01RightZ = (19 / 6);

    private const float goalZone02X = -10;

    private const float goalZone02LeftZ = (19 / 6);

    private const float goalZone02MiddleZ = 0;

    private const float goalZone02RightZ = -(19 / 6);

    public Text player01ScoreDisplay;

    private static int player01Goals;

    public Text player02ScoreDisplay;

    private static int player02Goals;

    private const float beginningXPositionOfBall = 0;

    private const float fixedYPositionOfBall = 0.5F;

    private const float beginningZPositionOfBall = 0;

    private void Awake()
    {
        gameOver.SetActive(false);
    }

    private void Start()
    {
        thisGoalZoneX = transform.position.x;
        thisGoalZoneZ = transform.position.z;

        player01ScoreDisplay.text = 
            Settings.settings.getProfileNamePlayerOne() + ": " + player01Goals;
        player02ScoreDisplay.text = 
            Settings.settings.getProfileNamePlayerTwo() + ": " + player02Goals;
    }

    private void OnCollisionEnter(Collision collision)
    {
        GameObject ball = collision.gameObject;

        ResetBall(ball);

        if (thisGoalZoneX == goalZone02X)
        {
            ++player01Goals;

            Statistics.statistics.UpdateStatisticIndependent(
                Settings.settings.getProfileIndexPlayerOne(),
                Statistics.winGoals,
                1);
            Statistics.statistics.UpdateStatisticIndependent(
                Settings.settings.getProfileIndexPlayerTwo(),
                Statistics.lossGoals,
                1);

            if (Mathf.Abs(thisGoalZoneZ - goalZone02RightZ) < 1)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.winGoalsLeft,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.lossGoalsRight,
                    1);
            }

            else if (thisGoalZoneZ == goalZone02MiddleZ)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.winGoalsMiddle,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.lossGoalsMiddle,
                    1);
            }

            else if (Mathf.Abs(thisGoalZoneZ - goalZone02LeftZ) < 1)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.winGoalsRight,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.lossGoalsLeft,
                    1);
            }

            player01ScoreDisplay.text = 
                Settings.settings.getProfileNamePlayerOne() + ": " + 
                player01Goals;

            if ((player01Goals % 5) == 0)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.wins,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.losses,
                    1);

                gameOver.SetActive(true);

                StartCoroutine(WaitAndEndGame());
            }
            else
            {
                StartCoroutine(WaitAndInitiateBallMovementRelativeToGoal(ball));
            }
        }

        else if (thisGoalZoneX == goalZone01X)
        {
            ++player02Goals;

            Statistics.statistics.UpdateStatisticIndependent(
                Settings.settings.getProfileIndexPlayerTwo(),
                Statistics.winGoals,
                1);
            Statistics.statistics.UpdateStatisticIndependent(
                Settings.settings.getProfileIndexPlayerOne(),
                Statistics.lossGoals,
                1);

            if (Mathf.Abs(thisGoalZoneZ - goalZone01RightZ) < 1)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.winGoalsLeft,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.lossGoalsRight,
                    1);
            }

            else if (thisGoalZoneZ == goalZone01MiddleZ)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.winGoalsMiddle,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.lossGoalsMiddle,
                    1);
            }

            else if (Mathf.Abs(thisGoalZoneZ - goalZone01LeftZ) < 1)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.winGoalsRight,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.lossGoalsLeft,
                    1);
            }

            player02ScoreDisplay.text = 
                Settings.settings.getProfileNamePlayerTwo() + ": " + 
                player02Goals;

            if ((player02Goals % 5) == 0)
            {
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerTwo(),
                    Statistics.wins,
                    1);
                Statistics.statistics.UpdateStatisticIndependent(
                    Settings.settings.getProfileIndexPlayerOne(),
                    Statistics.losses,
                    1);

                gameOver.SetActive(true);

                StartCoroutine(WaitAndEndGame());
            }
            else
            {
                StartCoroutine(WaitAndInitiateBallMovementRelativeToGoal(ball));
            }
        }

        Statistics.statistics.Save();
    }

    private void ResetBall(GameObject ball)
    {
        ResetBallPosition(ball);
    }

    private void ResetBallPosition(GameObject ball)
    {
        ball.transform.position = new Vector3(
            beginningXPositionOfBall, 
            fixedYPositionOfBall, 
            beginningZPositionOfBall);

        ball.GetComponent<Rigidbody>().velocity = Vector3.zero;
    }

    private IEnumerator WaitAndInitiateBallMovementRelativeToGoal(
        GameObject ball)
    {
        yield return new WaitForSecondsRealtime(1);

        InitiateBallMovementRelativeToGoal(ball);

    }

    private void InitiateBallMovementRelativeToGoal(GameObject ball)
    {
        float xVelocity = ball.GetComponent<BallBehaviour>().speed *
            Mathf.Sign(transform.position.x);
        ball.GetComponent<BallBehaviour>().InitiateBallMovement(xVelocity);
    }

    public IEnumerator WaitAndEndGame()
    {
        yield return new WaitForSecondsRealtime(1.5F);

        nullifyPlayScores();

        SceneManager.LoadScene(1, LoadSceneMode.Single);
    }

    public void nullifyPlayScores()
    {
        player01Goals = 0;
        player02Goals = 0;

        player01ScoreDisplay.text = 
            Settings.settings.getProfileNamePlayerOne() + ": " + 
            player01Goals;
        player02ScoreDisplay.text = 
            Settings.settings.getProfileNamePlayerTwo() + ": " + 
            player02Goals;
    }
}