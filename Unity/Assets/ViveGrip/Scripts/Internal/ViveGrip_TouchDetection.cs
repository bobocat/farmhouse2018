using UnityEngine;
using System.Collections.Generic;

public class ViveGrip_TouchDetection : MonoBehaviour {
  private List<ViveGrip_Object> collidingObjects = new List<ViveGrip_Object>();

    public Sensor sensor;       // send a message to the sensor if it is available


  void Start () {
        //    GetComponent<SphereCollider>().isTrigger = true;
        GetComponent<Collider>().isTrigger = true;

        if (GetComponent<Sensor>()) { sensor = GetComponent<Sensor>(); }
    }

  void OnTriggerEnter(Collider other) {
    ViveGrip_Object component = ActiveComponent(other.gameObject);
    if (component == null) { return; }
    collidingObjects.Add(component);

        if (sensor != null)
        {
            sensor.OnTouched();
        }
  }

  void OnTriggerExit(Collider other) {
    ViveGrip_Object component = ActiveComponent(other.gameObject);
    if (component == null) { return; }
    collidingObjects.Remove(component);
  }

  public GameObject NearestObject() {
    float closestDistance = Mathf.Infinity;
    GameObject touchedObject = null;
    foreach (ViveGrip_Object component in collidingObjects) {
      float distance = Vector3.Distance(transform.position, component.transform.position);
      if (distance < closestDistance) {
        touchedObject = component.gameObject;
        closestDistance = distance;
      }
    }
    return touchedObject;
  }

  ViveGrip_Object ActiveComponent(GameObject gameObject) {
    if (gameObject == null) { return null; } // Happens with Destroy() sometimes
    ViveGrip_Object component = ValidComponent(gameObject.transform);
    if (component == null) {
      component = ValidComponent(gameObject.transform.parent);
    }
    if (component != null) {
      return component;
    }
    return null;
  }

  ViveGrip_Object ValidComponent(Transform transform) {
    if (transform == null) { return null; }
    ViveGrip_Object component = transform.GetComponent<ViveGrip_Object>();
    if (component != null && component.enabled) { return component; }
    return null;
  }
}
