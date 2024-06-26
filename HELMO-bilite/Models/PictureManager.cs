using System.Security;
using HELMO_bilite.Models.Exceptions;

namespace HELMO_bilite.Models;

public class PictureManager
{
    private readonly IWebHostEnvironment _env;
    private readonly string _folderPicturePath;

    public PictureManager(IWebHostEnvironment env)
    {
        _env = env;
        _folderPicturePath = Path.Combine(_env.WebRootPath, "images");

    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="file">Représente un fichier reçu depuis une requête HTTP</param>
    /// <param name="userName">>le nom de l'utilisateur</param>
    /// <returns>le chemin du fichier publié dans le dossier "images" ou null</returns>
    /// <exception cref="HelmobiliteStorageException">Cette exception peut survenir si un problème est survenu lors de l'enregistrement du fichier</exception>
    public async Task<string> UploadAsync(IFormFile file, string userName)
    {
        if (file.Length == 0)
        {
            return null;
        }

        try
        {
            if (!Directory.Exists(_folderPicturePath))
            {
                Directory.CreateDirectory(_folderPicturePath);
            }
        }
        catch (DirectoryNotFoundException)
        {
            throw new HelmobiliteStorageException("Le chemin pointant vers le dossier n'a pu être trouvé");
        }
        catch (IOException e)
        {
            throw new HelmobiliteStorageException("Le dossier 'images' n'a pas pu être créé !");
        }


        var fileName = $"{userName}_{DateTime.UtcNow.Ticks}_{file.FileName}";
        var filePath = Path.Combine(_env.WebRootPath, "images", fileName);

        try
        {
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
        }
        catch (ArgumentException)
        {
            throw new HelmobiliteStorageException("Le fichier est invalide !");
        }
        catch (SecurityException)
        {
            throw new HelmobiliteStorageException("Vous n'avez pas les permissions pour enregistrer le fichier");
        }
        catch (FileNotFoundException)
        {
            throw new HelmobiliteStorageException("Le fichier n'existe pas");
        }
        catch (IOException)
        {
            throw new HelmobiliteStorageException("Une erreur est survenue lors de l'enregistrement du fichier");
        }
        // Conversion du chemin absolu (pour le stockage sur le système de fichiers du serveur) en chemin relatif pour l'affichage, manipulation,...
        filePath = GetRelativeImagePathForWeb(filePath);
        return filePath;
    }

    /// <summary>
    /// Cette méthode permet de récupérer une relative de l'image insérée
    /// qui est compatible avec une url WEB qui utilise généralement des barres
    /// obliques vers l'avant comme séparateur de chemin. Cependant, les systèmes d'exploitation
    /// ne respectent pas toujours ce même séparateur pour leurs fichiers locaux. Sous UNIX/Linux/macOs,
    /// le séparateur sera '/' à la différence de Windows qui utilise '\'. Ainsi, on s'assure du formater
    /// l'url correctement.
    /// </summary>
    /// <param name="absoluteImagePath"></param>
    /// <returns></returns>
    public string  GetRelativeImagePathForWeb(string absoluteImagePath)
    {
        // Permet d'extraire la racine (qui pointe vers le système de fichiers du serveur) de l'image
        string relativePath = Path.GetRelativePath(_env.WebRootPath, absoluteImagePath);
        string webRelativePath = relativePath.Replace(Path.DirectorySeparatorChar, '/');
        // Ajouter la barre oblique avant le chemin relatif, ce qui donnera /images/nomImage au lieu de images/nomImage
        return Path.Combine("~/", webRelativePath);
    }
    
}