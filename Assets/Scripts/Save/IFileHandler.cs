
public interface IFileHandler 
{

    PlayerSaveData Load();

    void Save(PlayerSaveData gameData);
}