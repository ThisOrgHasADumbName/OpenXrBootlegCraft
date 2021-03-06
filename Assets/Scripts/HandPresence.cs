using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.XR.Interaction.Toolkit;

public enum ControllerSide
{
	Left,
	Right
}

public class HandPresence : MonoBehaviour
{
	public InputDeviceCharacteristics controllerCharacteristics;
	public GameObject handModelPrefab;
	public ControllerSide controllerSide;

	private InputDevice targetDevice;
	private GameObject spawnedHandModel;
	private Animator handAnimator;
	private HandAction handAction;

	// Start is called before the first frame update
	void Start()
	{
		TryInitialize();
		handAction = PlayerController.Instance.getAction(controllerSide);
		handAction.interactor = transform.parent.parent.GetComponent<XRRayInteractor>();

	}

	void TryInitialize()
	{
		//Permet d'avoir la liste des controleurs.
		List<InputDevice> devices = new List<InputDevice>();

		//Permet d'aller chercher les contrôleurs
		InputDevices.GetDevicesWithCharacteristics(controllerCharacteristics, devices);

		//Vérifie si un controleur est initialiser
		if (devices.Count > 0)
		{
			targetDevice = devices[0];
			//Permet d'instantier un préfab.
			spawnedHandModel = Instantiate(handModelPrefab, transform);
			//Permet de récupérer l'Animator de l'objet qui vient d'être instantier
			handAnimator = spawnedHandModel.GetComponent<Animator>();
		}
	}
	//Fonction qui permet de faire animer les mains du joueur
	void UpdateHandAnimation()
	{
		//Fonction qui permet de récupérer la pression qui est mis sur le Trigger de la manette
		if (targetDevice.TryGetFeatureValue(CommonUsages.trigger, out float triggerValue))
		{
			//Permet d'animer la main selon la pression détecter
			handAnimator.SetFloat("Trigger", triggerValue);
			handAction.pressBumper = triggerValue;
			handAction.isPressingBumper = triggerValue > 0.3f; ;
		}
		else
		{
			handAnimator.SetFloat("Trigger", 0);
		}
		//Fonction qui permet de récupérer la pression qui est mis sur le Grip de la manette 
		if (targetDevice.TryGetFeatureValue(CommonUsages.grip, out float gripValue))
		{
			handAnimator.SetFloat("Grip", gripValue);
			handAction.grab = gripValue;
			handAction.isGrabbing = gripValue > 0.3f;
		}
		else
		{
			handAnimator.SetFloat("Grip", 0);
		}
	}

	// Update is called once per frame
	void Update()
	{
		//Véririe si un controlleur est initialiser.
		if (!targetDevice.isValid)
		{
			TryInitialize();
        }
        else
        {
			UpdateHandAnimation();
		}
	}
}