using UnityEngine;

public class PlanetSpawner : MonoBehaviour
{
    private GameObject DDOL;
    private Inventory inventory;
    private string planetInfo;

    [Header("PlanetSpawn")]
    [SerializeField] private GameObject naturePlanet;
    [SerializeField] private GameObject roundPlanet;
    [SerializeField] private GameObject robotPlanet;
    [SerializeField] private GameObject undeadPlanet;

    [Header("Volumes")]
    [SerializeField] private GameObject natureVolume;
    [SerializeField] private GameObject crabVolume;
    [SerializeField] private GameObject robotVolume;
    [SerializeField] private GameObject undeadVolume;
    void Awake()
    {
        InitPlanets();
        DDOL = GameObject.Find("DDOL");
        inventory = DDOL.GetComponent<Inventory>();
        planetInfo = inventory.planetInfo;

        Debug.Log(planetInfo);
        switch (planetInfo)
        {
            case "Nature Planet":
                naturePlanet.SetActive(true);
                natureVolume.SetActive(true);
                break;
            case "Robot Planet":
                robotPlanet.SetActive(true);
                robotVolume.SetActive(true);
                break;
            case "Round Planet":
                roundPlanet.SetActive(true);
                crabVolume.SetActive(true);
                break;
            case "Undead Planet":
                undeadPlanet.SetActive(true);
                undeadVolume.SetActive(true);
                break;
            default:
                break;
        }
    }

    void InitPlanets()
    {
        naturePlanet.SetActive(false);
        roundPlanet.SetActive(false);
        robotPlanet.SetActive(false);
        undeadPlanet.SetActive(false);
    }
    void InitVolumes()
    {
        natureVolume.SetActive(false);
        crabVolume.SetActive(false);
        robotVolume.SetActive(false);
        undeadVolume.SetActive(false);
    }
}
