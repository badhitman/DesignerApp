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
    "ContextPrefix" character(256),
    CONSTRAINT logs_pk PRIMARY KEY ("Id")
);

--CREATE INDEX logs_application_idx ON public.logs USING btree ("Application");
--CREATE INDEX logs_contextprefix_idx ON public.logs USING btree ("ContextPrefix");
--CREATE INDEX logs_level_idx ON public.logs USING btree ("Level");
--CREATE INDEX logs_logger_idx ON public.logs USING btree ("Logger");
--CREATE INDEX logs_timestamp_idx ON public.logs USING btree ("Timestamp");

ALTER TABLE IF EXISTS public.logs
    OWNER to nlog;