using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JaCaptei.Model.Entities;

using RepoDb.Attributes;

public class ImovelCaracteristicasExternas
{
    [Map("id")]
    public int Id { get; set; }

    [Map("idImovel")]
    public int IdImovel { get; set; }

    [Map("totalAndares")]
    public short TotalAndares { get; set; } = 0;

    [Map("totalElevadores")]
    public short TotalElevadores { get; set; } = 0;

    [Map("totalVagas")]
    public short TotalVagas { get; set; } = 0;

    [Map("academia")]
    public bool Academia { get; set; } = false;

    [Map("alarme")]
    public bool Alarme { get; set; } = false;

    [Map("cercaEletrica")]
    public bool CercaEletrica { get; set; } = false;

    [Map("churrasqueira")]
    public bool Churrasqueira { get; set; } = false;

    [Map("circuitoTV")]
    public bool CircuitoTV { get; set; } = false;

    [Map("elevador")]
    public bool Elevador { get; set; } = false;

    [Map("interfone")]
    public bool Interfone { get; set; } = false;

    [Map("jardim")]
    public bool Jardim { get; set; } = false;

    [Map("lavanderia")]
    public bool Lavanderia { get; set; } = false;

    [Map("portaoEletronico")]
    public bool PortaoEletronico { get; set; } = false;

    [Map("portaria24h")]
    public bool Portaria24h { get; set; } = false;

    [Map("sauna")]
    public bool Sauna { get; set; } = false;

    [Map("vaga")]
    public bool Vaga { get; set; } = false;
}

