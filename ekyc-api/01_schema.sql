-- Table: public.customer_ekyc

-- DROP TABLE IF EXISTS public.customer_ekyc;

CREATE TABLE IF NOT EXISTS public.customer_ekyc
(
    id uuid NOT NULL DEFAULT uuid_generate_v4(),
    customer_ref_no character varying(30) COLLATE pg_catalog."default" NOT NULL,
    ekyc_status ekyc_status_enum NOT NULL DEFAULT 'pending'::ekyc_status_enum,
    kyc_level kyc_level_enum NOT NULL DEFAULT 'basic'::kyc_level_enum,
    risk_score smallint NOT NULL DEFAULT 0,
    first_name character varying(80) COLLATE pg_catalog."default" NOT NULL,
    middle_name character varying(80) COLLATE pg_catalog."default",
    last_name character varying(80) COLLATE pg_catalog."default" NOT NULL,
    dob date NOT NULL,
    gender gender_enum NOT NULL,
    mobile character varying(15) COLLATE pg_catalog."default" NOT NULL,
    email character varying(255) COLLATE pg_catalog."default" NOT NULL,
    pan_masked character varying(10) COLLATE pg_catalog."default",
    aadhaar_masked character varying(20) COLLATE pg_catalog."default",
    document_type doc_type_enum,
    document_number_masked character varying(30) COLLATE pg_catalog."default",
    verification_provider character varying(100) COLLATE pg_catalog."default",
    verification_reference_id character varying(150) COLLATE pg_catalog."default",
    address_line1 character varying(200) COLLATE pg_catalog."default",
    address_line2 character varying(200) COLLATE pg_catalog."default",
    city character varying(80) COLLATE pg_catalog."default",
    district character varying(80) COLLATE pg_catalog."default",
    state character varying(80) COLLATE pg_catalog."default",
    pincode character varying(10) COLLATE pg_catalog."default",
    country character varying(60) COLLATE pg_catalog."default" NOT NULL DEFAULT 'India'::character varying,
    org_id uuid NOT NULL,
    org_name character varying(200) COLLATE pg_catalog."default",
    org_gstin character varying(20) COLLATE pg_catalog."default",
    org_email character varying(255) COLLATE pg_catalog."default",
    org_phone character varying(15) COLLATE pg_catalog."default",
    photo_base64 text COLLATE pg_catalog."default",
    photo_mime_type character varying(30) COLLATE pg_catalog."default",
    photo_size_bytes integer,
    created_at timestamp with time zone NOT NULL DEFAULT now(),
    updated_at timestamp with time zone NOT NULL DEFAULT now(),
    created_by character varying(100) COLLATE pg_catalog."default",
    updated_by character varying(100) COLLATE pg_catalog."default",
    is_active boolean NOT NULL DEFAULT true,
    consent_taken boolean NOT NULL DEFAULT false,
    consent_ts timestamp with time zone,
    search_vector tsvector,
    CONSTRAINT pk_customer_ekyc PRIMARY KEY (id),
    CONSTRAINT uq_customer_email UNIQUE (email),
    CONSTRAINT uq_customer_mobile UNIQUE (mobile),
    CONSTRAINT uq_customer_ref_no UNIQUE (customer_ref_no),
    CONSTRAINT customer_ekyc_risk_score_check CHECK (risk_score >= 0 AND risk_score <= 100)
)

TABLESPACE pg_default;

ALTER TABLE IF EXISTS public.customer_ekyc
    OWNER to postgres;

COMMENT ON TABLE public.customer_ekyc
    IS 'eKYC master table — Ranlab_ekyc';

COMMENT ON COLUMN public.customer_ekyc.id
    IS 'Primary key UUID v4';

COMMENT ON COLUMN public.customer_ekyc.customer_ref_no
    IS 'Unique human-readable reference e.g. REF0000000001';

COMMENT ON COLUMN public.customer_ekyc.ekyc_status
    IS 'KYC state: pending | verified | rejected';

COMMENT ON COLUMN public.customer_ekyc.kyc_level
    IS 'Depth of KYC: basic | full';

COMMENT ON COLUMN public.customer_ekyc.risk_score
    IS 'Risk score 0–100';

COMMENT ON COLUMN public.customer_ekyc.mobile
    IS 'Unique mobile number';

COMMENT ON COLUMN public.customer_ekyc.email
    IS 'Unique email address';

COMMENT ON COLUMN public.customer_ekyc.pan_masked
    IS 'Masked PAN e.g. ABCPX1234X';

COMMENT ON COLUMN public.customer_ekyc.aadhaar_masked
    IS 'Masked Aadhaar e.g. XXXX-XXXX-4321';

COMMENT ON COLUMN public.customer_ekyc.org_id
    IS 'Organisation reference UUID';

COMMENT ON COLUMN public.customer_ekyc.photo_base64
    IS 'Customer photo as Base64 string';

COMMENT ON COLUMN public.customer_ekyc.photo_size_bytes
    IS 'Original byte size before Base64 encoding';

COMMENT ON COLUMN public.customer_ekyc.is_active
    IS 'Soft-delete: FALSE = logically deleted';

COMMENT ON COLUMN public.customer_ekyc.consent_taken
    IS 'TRUE if customer gave data processing consent';

COMMENT ON COLUMN public.customer_ekyc.consent_ts
    IS 'Timestamp when consent was recorded';

COMMENT ON COLUMN public.customer_ekyc.search_vector
    IS 'Auto-maintained tsvector for full-text search';
-- Index: idx_ekyc_active

-- DROP INDEX IF EXISTS public.idx_ekyc_active;

CREATE INDEX IF NOT EXISTS idx_ekyc_active
    ON public.customer_ekyc USING btree
    (created_at DESC NULLS FIRST)
    TABLESPACE pg_default
    WHERE is_active = true;
-- Index: idx_ekyc_fts

-- DROP INDEX IF EXISTS public.idx_ekyc_fts;

CREATE INDEX IF NOT EXISTS idx_ekyc_fts
    ON public.customer_ekyc USING gin
    (search_vector)
    TABLESPACE pg_default;
-- Index: idx_ekyc_org_id

-- DROP INDEX IF EXISTS public.idx_ekyc_org_id;

CREATE INDEX IF NOT EXISTS idx_ekyc_org_id
    ON public.customer_ekyc USING btree
    (org_id ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: idx_ekyc_pagination

-- DROP INDEX IF EXISTS public.idx_ekyc_pagination;

CREATE INDEX IF NOT EXISTS idx_ekyc_pagination
    ON public.customer_ekyc USING btree
    (created_at DESC NULLS FIRST, id DESC NULLS FIRST)
    TABLESPACE pg_default;
-- Index: idx_ekyc_risk_score

-- DROP INDEX IF EXISTS public.idx_ekyc_risk_score;

CREATE INDEX IF NOT EXISTS idx_ekyc_risk_score
    ON public.customer_ekyc USING btree
    (risk_score ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: idx_ekyc_status

-- DROP INDEX IF EXISTS public.idx_ekyc_status;

CREATE INDEX IF NOT EXISTS idx_ekyc_status
    ON public.customer_ekyc USING btree
    (ekyc_status ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: idx_ekyc_trgm_email

-- DROP INDEX IF EXISTS public.idx_ekyc_trgm_email;

CREATE INDEX IF NOT EXISTS idx_ekyc_trgm_email
    ON public.customer_ekyc USING gin
    (email COLLATE pg_catalog."default" gin_trgm_ops)
    TABLESPACE pg_default;
-- Index: idx_ekyc_trgm_mobile

-- DROP INDEX IF EXISTS public.idx_ekyc_trgm_mobile;

CREATE INDEX IF NOT EXISTS idx_ekyc_trgm_mobile
    ON public.customer_ekyc USING gin
    (mobile COLLATE pg_catalog."default" gin_trgm_ops)
    TABLESPACE pg_default;
-- Index: idx_ekyc_trgm_org_name

-- DROP INDEX IF EXISTS public.idx_ekyc_trgm_org_name;

CREATE INDEX IF NOT EXISTS idx_ekyc_trgm_org_name
    ON public.customer_ekyc USING gin
    (org_name COLLATE pg_catalog."default" gin_trgm_ops)
    TABLESPACE pg_default;
-- Index: idx_ekyc_trgm_ref

-- DROP INDEX IF EXISTS public.idx_ekyc_trgm_ref;

CREATE INDEX IF NOT EXISTS idx_ekyc_trgm_ref
    ON public.customer_ekyc USING gin
    (customer_ref_no COLLATE pg_catalog."default" gin_trgm_ops)
    TABLESPACE pg_default;
-- Index: ix_customer_created_at

-- DROP INDEX IF EXISTS public.ix_customer_created_at;

CREATE INDEX IF NOT EXISTS ix_customer_created_at
    ON public.customer_ekyc USING btree
    (created_at DESC NULLS FIRST)
    TABLESPACE pg_default;
-- Index: ix_customer_org_id

-- DROP INDEX IF EXISTS public.ix_customer_org_id;

CREATE INDEX IF NOT EXISTS ix_customer_org_id
    ON public.customer_ekyc USING btree
    (org_id ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: ix_customer_search

-- DROP INDEX IF EXISTS public.ix_customer_search;

CREATE INDEX IF NOT EXISTS ix_customer_search
    ON public.customer_ekyc USING gin
    (((((((((COALESCE(first_name, ''::character varying)::text || ' '::text) || COALESCE(last_name, ''::character varying)::text) || ' '::text) || COALESCE(email, ''::character varying)::text) || ' '::text) || COALESCE(mobile, ''::character varying)::text) || ' '::text) || COALESCE(org_name, ''::character varying)::text) COLLATE pg_catalog."default" gin_trgm_ops)
    TABLESPACE pg_default;
-- Index: ix_customer_status

-- DROP INDEX IF EXISTS public.ix_customer_status;

CREATE INDEX IF NOT EXISTS ix_customer_status
    ON public.customer_ekyc USING btree
    (ekyc_status ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: ux_customer_email

-- DROP INDEX IF EXISTS public.ux_customer_email;

CREATE UNIQUE INDEX IF NOT EXISTS ux_customer_email
    ON public.customer_ekyc USING btree
    (email COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;
-- Index: ux_customer_mobile

-- DROP INDEX IF EXISTS public.ux_customer_mobile;

CREATE UNIQUE INDEX IF NOT EXISTS ux_customer_mobile
    ON public.customer_ekyc USING btree
    (mobile COLLATE pg_catalog."default" ASC NULLS LAST)
    TABLESPACE pg_default;

-- Trigger: trig_customer_ekyc_search_vector

-- DROP TRIGGER IF EXISTS trig_customer_ekyc_search_vector ON public.customer_ekyc;

CREATE OR REPLACE TRIGGER trig_customer_ekyc_search_vector
    BEFORE INSERT OR UPDATE 
    ON public.customer_ekyc
    FOR EACH ROW
    EXECUTE FUNCTION public.fn_update_search_vector();

-- Trigger: trig_customer_ekyc_updated_at

-- DROP TRIGGER IF EXISTS trig_customer_ekyc_updated_at ON public.customer_ekyc;

CREATE OR REPLACE TRIGGER trig_customer_ekyc_updated_at
    BEFORE UPDATE 
    ON public.customer_ekyc
    FOR EACH ROW
    EXECUTE FUNCTION public.fn_set_updated_at();