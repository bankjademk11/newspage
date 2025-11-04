/// <reference types="vite/client" />

interface ImportMetaEnv {
  readonly VITE_NEWS_API_KEY: string
  readonly VITE_API_BASE_URL: string
  readonly VITE_APP_TITLE: string
  readonly more: string
}

interface ImportMeta {
  readonly env: ImportMetaEnv
}
