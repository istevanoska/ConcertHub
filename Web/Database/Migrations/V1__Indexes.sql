-- Supplemental, idempotent raw-SQL migration applied by Evolve on top of the EF-created schema.
-- Adds helpful indexes for the most common lookups.

CREATE INDEX IF NOT EXISTS IX_Tickets_ConcertId ON Tickets (ConcertId);
CREATE INDEX IF NOT EXISTS IX_Tickets_UserId ON Tickets (UserId);
CREATE INDEX IF NOT EXISTS IX_Performances_ConcertId ON Performances (ConcertId);
CREATE INDEX IF NOT EXISTS IX_Concerts_StartTime ON Concerts (StartTime);
CREATE INDEX IF NOT EXISTS IX_InboundEventEntries_Status ON InboundEventEntries (Status);
