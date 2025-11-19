using System;
using System.Collections.Generic;

namespace CegautokAPI.Models;

public partial class Kikuldottjarmu
{
    public int Id { get; set; }

    public int GepJarmuId { get; set; }

    public int KikuldetesId { get; set; }

    public int Sofor { get; set; }

    public virtual Gepjarmu? GepJarmu { get; set; } = null!;

    public virtual Kikuldete? Kikuldetes { get; set; } = null!;

    public virtual User? SoforNavigation { get; set; } = null!;
}
