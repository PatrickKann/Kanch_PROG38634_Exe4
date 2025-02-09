using UnityEngine;
using System.Collections;

using UnityEngine.UI;

public class Radar : MonoBehaviour
{
	[Range(10f, 50.0f)]
	public float insideRadarDistance = 20;

	[Range(1f, 10.0f)]
	public float blipSizePercentage = 5;

	[Range(0.5f, 3.0f)] //to scale the player bigger/smaller
	public float blipSizeScale = 1f;

	public GameObject rawImageBlipPlayer;
	public GameObject rawImageBlipPedestrian;
	public GameObject rawImageBlipCar;

	private RawImage rawImageRadarBackground;
	private Transform playerTransform;
	private float radarWidth;
	private float radarHeight;
	private float blipHeight;
	private float blipWidth;


	void Start (){
		playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
		rawImageRadarBackground = GetComponent<RawImage>();

		radarWidth = rawImageRadarBackground.rectTransform.rect.width;
		radarHeight = rawImageRadarBackground.rectTransform.rect.height;

		blipHeight = radarHeight * blipSizePercentage/100;
		blipWidth = radarWidth * blipSizePercentage/100;
	}

	void Update (){
		RemoveAllBlips();
		FindAndDisplayBlipsForTag("Player", rawImageBlipPlayer);
		FindAndDisplayBlipsForTag("Pedestrian", rawImageBlipPedestrian);
		FindAndDisplayBlipsForTag("Car", rawImageBlipCar);
	}

	private void FindAndDisplayBlipsForTag(string tag, GameObject prefabBlip){
		Vector3 playerPos = playerTransform.position;
		GameObject[] targets = GameObject.FindGameObjectsWithTag(tag);

		blipHeight = radarHeight * blipSizePercentage / 100;
		blipWidth = radarWidth * blipSizePercentage / 100;

		foreach (GameObject target in targets)  
		{
			Vector3 targetPos = target.transform.position;
			float distanceToTarget = Vector3.Distance(targetPos, playerPos);
			if( (distanceToTarget <= insideRadarDistance) ){
				Vector3 normalisedTargetPosiiton = NormaisedPosition(playerPos, targetPos);
				//print (normalisedTargetPosiiton);
				Vector2 blipPosition = CalculateBlipPosition(normalisedTargetPosiiton);

				if (target.tag == "Player")
					DrawBlip(blipPosition, prefabBlip, blipWidth * blipSizeScale, blipHeight * blipSizeScale);
				else
					DrawBlip(blipPosition, prefabBlip);
			}
		}
	}

	private void RemoveAllBlips(){
		GameObject[] blips = GameObject.FindGameObjectsWithTag("Blip");	
		foreach (GameObject blip in blips)  
			Destroy(blip);
	}

	private Vector3 NormaisedPosition(Vector3 playerPos, Vector3 targetPos){
		float normalisedyTargetX = (targetPos.x - playerPos.x)/insideRadarDistance;
		float normalisedyTargetZ = (targetPos.z - playerPos.z)/insideRadarDistance;
		return new Vector3(normalisedyTargetX, 0, normalisedyTargetZ);
	}
	
	private Vector2 CalculateBlipPosition(Vector3 targetPos){
		// find angle from player to target
		float angleToTarget = Mathf.Atan2(targetPos.x, targetPos.z) * Mathf.Rad2Deg;
		
		// direction player facing
		float anglePlayer = playerTransform.eulerAngles.y;
		
		// subtract player angle, to get relative angle to object
		// subtract 90
		// (so 0 degrees (same direction as player) is UP)
		float angleRadarDegrees =  angleToTarget - anglePlayer - 90;
		
		// calculate (x,y) position given angle and distance
		float normalisedDistanceToTarget = targetPos.magnitude;
		float angleRadians = angleRadarDegrees * Mathf.Deg2Rad;
		float blipX = normalisedDistanceToTarget * Mathf.Cos(angleRadians);
		float blipY = normalisedDistanceToTarget * Mathf.Sin(angleRadians);	
		
		// scale blip position according to radar size
		blipX *= radarWidth/2;
		blipY *= radarHeight/2;
		
		// offset blip position relative to radar center
		blipX += radarWidth/2;
		blipY += radarHeight/2;

		return new Vector2(blipX, blipY);
	}

	private void DrawBlip(Vector2 pos, GameObject blipPrefab){
		GameObject blipGO = (GameObject)Instantiate(blipPrefab);
		blipGO.transform.SetParent(transform.parent);
		RectTransform rt = blipGO.GetComponent<RectTransform>();
		rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, pos.x, blipWidth);
		rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos.y, blipHeight);
	}

	private void DrawBlip(Vector2 pos, GameObject blipPrefab, float w, float h)
	{
		//for player
		GameObject blipGO = (GameObject)Instantiate(blipPrefab);
		blipGO.transform.SetParent(transform.parent);
		RectTransform rt = blipGO.GetComponent<RectTransform>();
		rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Right, pos.x, w);
		rt.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, pos.y, h);
	}
}