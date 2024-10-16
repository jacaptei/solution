﻿using JaCaptei.Model;

using Npgsql;
using RepoDb;


namespace JaCaptei.Application.DAL;

public class DBcontext: IDisposable
{

    public string CS = Config.settings?.DBconnectionString;

    private static bool mapped { get; set; } = false;

    public NpgsqlConnection conn = null;

    public DBcontext()
    {
        Map();
    }

    public DBcontext(NpgsqlConnection _conn)
    {
        Map();
        conn = _conn;
    }

    public NpgsqlConnection GetConn()
    {
        //if(conn is null)
        conn = new NpgsqlConnection(GetConnectionString());
        return conn;
    }

    public String GetConnectionString()
    {

        if (String.IsNullOrWhiteSpace(CS))
            throw new Exception("PGSQL Connection String undefined.");

        return CS;

    }

    public void Map()
    {
        if (!mapped)
        {
            ClassMapper.Clear();
            //PropertyMapper.Remove<Shared.Model.User>(e => e.login);
            //PropertyMapper.Remove<Shared.Model.User>(e => e.grupo);
            // TABELA PRECISA COMEÇAR COM [DBO].[TABELA] ex: dbo.ParticipanteRodada
            // PARA FUNCIONAR O (AUTO) MAPPING

            // usando a nova lib
            // Microsoft.Data.SqlClient
            // https://repodb.net/release/sqlserver

            //ClassMapper.Add<UserDriver>("userdriver");
            mapped = true;
        }
    }

    public void Dispose()
    {
        conn?.Close();
        conn?.Dispose();
    }
}




/*
 * --https://repodb.net/feature/implicitmapping
 * To remove the mapping, use the Remove() method.

PropertyHandlerMapper.Remove<Customer>(e => e.Address);

PropertyMapper.Add<Customer>(e => e.FirstName, "[FName]");
var firstName = PropertyMapper.Get<Customer>(e => e.FirstName);



*/