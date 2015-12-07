CREATE TABLE [dbo].[StateTimeZone] (
    [StateTimeZoneID]         BIGINT IDENTITY (1, 1) NOT NULL,
    [StateID]                 BIGINT NOT NULL,
    [InternationalTimeZoneID] BIGINT NOT NULL,
    CONSTRAINT [PK_StateTimeZone] PRIMARY KEY CLUSTERED ([StateTimeZoneID] ASC),
    CONSTRAINT [FK_StateTimeZone_State] FOREIGN KEY ([StateID]) REFERENCES [dbo].[State] ([StateID]),
    CONSTRAINT [FK_StateTimeZone_TimeZone] FOREIGN KEY ([InternationalTimeZoneID]) REFERENCES [dbo].[InternationalTimeZone] ([InternationalTimeZoneID]),
    CONSTRAINT [IX_StateInternationalTimeZone] UNIQUE NONCLUSTERED ([StateID] ASC, [InternationalTimeZoneID] ASC)
);

