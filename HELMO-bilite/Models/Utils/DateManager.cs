namespace HELMO_bilite.Models.Utils;

public class DateManager
{
    /// <summary>
    /// Cette méthode a pour but de nous donner la date de début et fin d'une semaine précise en fonction
    /// de la valeur du décalage de semaine. On considère toujours que la valeur de l'offset est à 0 pour la semaine
    /// courante. Elle sera négative pour chaque semaine écoulée dans le passée et positiée pour chaque semaine dans le futur.
    /// </summary>
    /// <param name="weekOffset">la valeur du décalage hebdomadaire</param>
    /// <returns>la date de début et fin d'une semaine en fonction du décalage hebdomadaire</returns>
    public static (DateTime Start, DateTime End) GetWeekRange(int weekOffset)
    {
        DateTime today = DateTime.Today;
        // Calculer le nombre de jours à ajouter à la date d'aujourd'hui pour obtenir le premier jour de la semainz
        int daysToFirstDayOfWeek = (int)DayOfWeek.Monday - (int)today.DayOfWeek;

        // prendre en compte le décalage de semaine en compte afin de savoir le nombre de jours écoulés à
        // notre date actuelle.
        DateTime startOfWeek = today.AddDays(daysToFirstDayOfWeek + (weekOffset * 7));
        // dernier jour de la semaine
        DateTime endOfWeek = startOfWeek.AddDays(6);

        return (startOfWeek, endOfWeek);
    }

}