--DROP TABLE IF EXISTS "SessaoUsuario";

CREATE	TABLE "SessaoUsuario"(
	"id"                SERIAL PRIMARY KEY,         
    "sessionId"         UUID NOT NULL UNIQUE,        
    "idParceiro"            INT NOT NULL,
    "tokenJWT"          TEXT NOT NULL,
    "ipAddress"         VARCHAR(45),
    "userAgent"         TEXT,
    "createdAt"         TIMESTAMPTZ NOT NULL DEFAULT NOW(),
    "expiresAt"         TIMESTAMPTZ,
    "lastAccessedAt"    TIMESTAMPTZ,
    "isRevoked"         BOOLEAN DEFAULT FALSE,
    "revokedAt"         TIMESTAMPTZ,
    "revokedByIp"       VARCHAR(45),
    "createdByIp"       VARCHAR(45),
    "replacedBySession" UUID,
    FOREIGN KEY ("userId") REFERENCES "Parceiro"(id)
);