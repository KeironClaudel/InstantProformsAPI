-- Future Supabase RLS policy for direct client access with multitenancy.
-- Do NOT apply this while the current architecture still routes all database access
-- through the backend .NET application.
--
-- Prerequisites:
--   auth.jwt()->'app_metadata'->>'company_id' must contain the tenant UUID
--   auth.jwt()->'app_metadata'->>'app_role'   must contain Owner/Admin/Employee

BEGIN;

create or replace function public.current_company_id()
returns uuid
language sql
stable
as $$
  select nullif(auth.jwt() -> 'app_metadata' ->> 'company_id', '')::uuid
$$;

create or replace function public.current_app_role()
returns text
language sql
stable
as $$
  select coalesce(auth.jwt() -> 'app_metadata' ->> 'app_role', '')
$$;

-- Remove the current deny-all policies first.
drop policy if exists "deny_all_anon_authenticated" on public."Clients";
drop policy if exists "deny_all_anon_authenticated" on public."Companies";
drop policy if exists "deny_all_anon_authenticated" on public."CompanySettings";
drop policy if exists "deny_all_anon_authenticated" on public."PasswordResetTokens";
drop policy if exists "deny_all_anon_authenticated" on public."ProformItems";
drop policy if exists "deny_all_anon_authenticated" on public."ProformShareTokens";
drop policy if exists "deny_all_anon_authenticated" on public."Proforms";
drop policy if exists "deny_all_anon_authenticated" on public."RefreshTokens";
drop policy if exists "deny_all_anon_authenticated" on public."Roles";
drop policy if exists "deny_all_anon_authenticated" on public."StoredFiles";
drop policy if exists "deny_all_anon_authenticated" on public."Users";
drop policy if exists "deny_all_anon_authenticated" on public."__EFMigrationsHistory";

-- Companies: read own company only. Keep mutations away from direct client access.
create policy "company_select_own"
on public."Companies"
for select
to authenticated
using ("Id" = public.current_company_id());

-- Company settings: read own row, mutate only as Owner/Admin.
create policy "company_settings_select_own"
on public."CompanySettings"
for select
to authenticated
using ("CompanyId" = public.current_company_id());

create policy "company_settings_modify_admin"
on public."CompanySettings"
for all
to authenticated
using (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin')
)
with check (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin')
);

-- Clients: full access inside the tenant for app roles.
create policy "clients_select_own_company"
on public."Clients"
for select
to authenticated
using ("CompanyId" = public.current_company_id());

create policy "clients_modify_own_company"
on public."Clients"
for all
to authenticated
using (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin', 'Employee')
)
with check (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin', 'Employee')
);

-- Proforms: full access inside the tenant for app roles.
create policy "proforms_select_own_company"
on public."Proforms"
for select
to authenticated
using ("CompanyId" = public.current_company_id());

create policy "proforms_modify_own_company"
on public."Proforms"
for all
to authenticated
using (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin', 'Employee')
)
with check (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin', 'Employee')
);

-- Proform items inherit tenant access through the parent proform.
create policy "proform_items_select_by_parent"
on public."ProformItems"
for select
to authenticated
using (
  exists (
    select 1
    from public."Proforms" p
    where p."Id" = "ProformId"
      and p."CompanyId" = public.current_company_id()
  )
);

create policy "proform_items_modify_by_parent"
on public."ProformItems"
for all
to authenticated
using (
  exists (
    select 1
    from public."Proforms" p
    where p."Id" = "ProformId"
      and p."CompanyId" = public.current_company_id()
      and public.current_app_role() in ('Owner', 'Admin', 'Employee')
  )
)
with check (
  exists (
    select 1
    from public."Proforms" p
    where p."Id" = "ProformId"
      and p."CompanyId" = public.current_company_id()
      and public.current_app_role() in ('Owner', 'Admin', 'Employee')
  )
);

-- Stored files: only tenant-owned rows, and mutations only for Owner/Admin.
create policy "stored_files_select_own_company"
on public."StoredFiles"
for select
to authenticated
using ("CompanyId" = public.current_company_id());

create policy "stored_files_modify_admin"
on public."StoredFiles"
for all
to authenticated
using (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin')
)
with check (
  "CompanyId" = public.current_company_id()
  and public.current_app_role() in ('Owner', 'Admin')
);

-- Internal/security tables remain blocked from direct client access.
create policy "roles_deny_authenticated"
on public."Roles"
for all
to authenticated
using (false)
with check (false);

create policy "users_deny_authenticated"
on public."Users"
for all
to authenticated
using (false)
with check (false);

create policy "refresh_tokens_deny_authenticated"
on public."RefreshTokens"
for all
to authenticated
using (false)
with check (false);

create policy "password_reset_tokens_deny_authenticated"
on public."PasswordResetTokens"
for all
to authenticated
using (false)
with check (false);

create policy "proform_share_tokens_deny_authenticated"
on public."ProformShareTokens"
for all
to authenticated
using (false)
with check (false);

create policy "ef_history_deny_authenticated"
on public."__EFMigrationsHistory"
for all
to authenticated
using (false)
with check (false);

COMMIT;
