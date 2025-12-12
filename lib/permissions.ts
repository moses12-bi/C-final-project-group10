export type PermissionMap = Record<string, boolean>;

export function getStoredPermissions(): PermissionMap {
  if (typeof window === 'undefined') return {};
  try {
    const raw = localStorage.getItem('permissions');
    if (!raw) return {};
    return JSON.parse(raw) as PermissionMap;
  } catch {
    return {};
  }
}

export function hasPermission(code: string): boolean {
  const perms = getStoredPermissions();
  return perms[code] === true;
}
