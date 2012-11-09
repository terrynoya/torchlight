#pragma strict

private var agents : NavMeshAgent[];
public var marker : Transform;
public var rayCamera : Camera;

function Start () {
	agents = FindObjectsOfType(NavMeshAgent) as NavMeshAgent[];
}

function UdateAgentTargets(targetPosition : Vector3) {
	for(var agent : NavMeshAgent in agents) {
		agent.destination = targetPosition;
	}
}

function Update () {
  var button : int = 0;
  if(Input.GetMouseButtonDown(button)) {
    var ray : Ray = rayCamera.ScreenPointToRay(Input.mousePosition);
    var hitInfo : RaycastHit;
    if (Physics.Raycast(ray.origin, ray.direction, hitInfo)) {
      var targetPosition : Vector3 = hitInfo.point;
      UdateAgentTargets(targetPosition);
			marker.position = targetPosition + Vector3(0,1,0);
    }
  }
}
