CREATE TABLE public.logs
(
    "Id" integer GENERATED ALWAYS AS IDENTITY NOT NULL,
    "Application" character(256),
    "Timestamp" timestamp without time zone NOT NULL,
    "Level" character(128) NOT NULL,
    "Message" varchar,
    "Logger" character(256),
    "Callsite" character(256),
    "Exception" varchar,
    CONSTRAINT logs_pk PRIMARY KEY ("Id")
);

ALTER TABLE IF EXISTS public.logs
    OWNER to nlog;