{{/*
Expand the name of the chart.
*/}}
{{- define "hmac-manager.name" -}}
{{- default .Chart.Name .Values.nameOverride | trunc 63 | trimSuffix "-" }}
{{- end }}

{{/*
Create a default fully qualified app name.
*/}}
{{- define "hmac-manager.fullname" -}}
{{- if .Values.fullnameOverride }}
{{- .Values.fullnameOverride | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- $name := default .Chart.Name .Values.nameOverride }}
{{- if contains $name .Release.Name }}
{{- .Release.Name | trunc 63 | trimSuffix "-" }}
{{- else }}
{{- printf "%s-%s" .Release.Name $name | trunc 63 | trimSuffix "-" }}
{{- end }}
{{- end }}
{{- end }}

{{/*
Common labels.
*/}}
{{- define "hmac-manager.labels" -}}
helm.sh/chart: {{ printf "%s-%s" .Chart.Name .Chart.Version | replace "+" "_" | trunc 63 | trimSuffix "-" }}
{{ include "hmac-manager.selectorLabels" . }}
{{- if .Chart.AppVersion }}
app.kubernetes.io/version: {{ .Chart.AppVersion | quote }}
{{- end }}
app.kubernetes.io/managed-by: {{ .Release.Service }}
{{- end }}

{{/*
Selector labels.
*/}}
{{- define "hmac-manager.selectorLabels" -}}
app.kubernetes.io/name: {{ include "hmac-manager.name" . }}
app.kubernetes.io/instance: {{ .Release.Name }}
{{- end }}

{{/*
Cluster-internal address of the bundled Redis instance.
*/}}
{{- define "hmac-manager.redisAddress" -}}
{{- printf "%s-redis:6379" (include "hmac-manager.fullname" .) -}}
{{- end }}

{{/*
Build the HmacManager policy config JSON from .Values.policies.
The result is a compact JSON string suitable for use as a ConfigMap value.
Private keys are intentionally excluded — they are injected at runtime via
secretKeyRef env vars so they never appear in the ConfigMap.
*/}}
{{- define "hmac-manager.policyConfig" -}}
{{- $root := . -}}
{{- $policies := list -}}
{{- range .Values.policies -}}
  {{- $nonce := dict
    "CacheType"       (ternary "Distributed" "Memory" $root.Values.redis.enabled)
    "MaxAgeInSeconds" (dig "nonce" "maxAgeInSeconds" 60 .) -}}
  {{- $algorithms := dict
    "ContentHashAlgorithm" (dig "algorithms" "contentHash" "SHA256" .)
    "SigningHashAlgorithm"  (dig "algorithms" "signingHash" "HMACSHA256" .) -}}
  {{- $keys := dict "PublicKey" .publicKey -}}
  {{- $p := dict "Name" .name "Keys" $keys "Algorithms" $algorithms "Nonce" $nonce -}}
  {{- if .schemes -}}
    {{- $schemeList := list -}}
    {{- range .schemes -}}
      {{- $headerList := list -}}
      {{- range .headers -}}
        {{- $headerList = append $headerList (dict "Name" .name "ClaimType" (default .name .claimType)) -}}
      {{- end -}}
      {{- $schemeList = append $schemeList (dict "Name" .name "Headers" $headerList) -}}
    {{- end -}}
    {{- $_ := set $p "Schemes" $schemeList -}}
  {{- end -}}
  {{- $policies = append $policies $p -}}
{{- end -}}
{{- dict "HmacManager" $policies | toJson -}}
{{- end -}}
