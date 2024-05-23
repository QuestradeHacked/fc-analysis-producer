## Engineering practice
- [X] Application repository has to have a README.md
    - [X] Readme should describe purpose of the service
    - [X] Readme should describe usage of datastores
    - [X] Readme should describe any other resiliency patterns/degradation features
    - [X] Anything else that is critical to understanding the service (eg. external partner reliance, caching mechanisms)
    - [X] Readme should contain diagram of service and dependencies
    - [X] Teams Slack Channel Link
    - [X] Teams Email group
 - [X] Exposed APIs must be published on [API portal](http://api-portal.preprod.google.questech.io/) (and [API-Registry](http://git.questrade.com/msa/api-registry)). Here are instructions on how to do [this](https://api-portal.uat.questrade.com/contribute)
 - [X] Code is reviewed by another engineer
    - [X] Usually another team member but enterprise architects can also be engaged for this
    - [X] Code reviews done on a MR basis. Smaller the better
- [X] Project must implement unit tests 
    - [X] Quality/coverage of unit tests should be part of code review (per feature/MR)
    - Coverage % to be announced later
- [X] Project must be using supported languages and libraries
    - [X] Eg. nothing should be EOL at time of shipping
- [X] All secrets in Vault. All non-secret environment variables must be committed to Git. 
    - Details [here](https://confluence/display/QIOS/Configuration+and+secret+management+for+Cloud+Native+Applications)

## Operational Readiness
- [ ] Service level indicators (SLI) and Service level objectives (SLO) need to be identified
    - [ ] SLOs should be set by the business with the end user experience being the focus
    - [ ] Start with something realistic and simple (eg. closer to 3 than 10 SLOs, easily identifiable things)
    - [ ] Production support team can help with this and see this [doc](https://landing.google.com/sre/sre-book/chapters/service-level-objectives/) for basics
- [X] Application must have been deployed successfully to SIT before moving to higher environments
- [X] Idempotent for restarts (a service can be killed and started multiple times)
- [X] Idempotent for scaling up/down (a service can be autoscaled to multiple instances)
- [X] Application should not have any background job running in the same process as the normal runtime (eg: timed cache clean or DB record purge)
    - Exception: Jobs like message subscribers, which should also gracefully handle and understand app healthiness state and lifecycle
- [ ] Capacity planning must be done before moving to production
    - See [docs](http://git.questrade.com/infra/docs/blob/master/paas.config.md#kubernetes-configuration)
- [X] Alerts must be created (by the developing team) for production incidents. Examples:
    - [X] Service is down
    - [X] Amount of 5xx s is high
    - [X] Service is throwing exceptions

## Observability
- [X] Service needs to have a dashboard in Datadog ([Dashboard](https://app.datadoghq.com/dashboard/vsr-vv4-fcg) is not mandatory to follow but might help you get started - remember to configure for all environments!)
    - [X] Showing health of the service (error rates, response times, usage)
    - [X] Showing resource usage (CPU/MEM metrics etc)
    - [X] Datastore (Redis, MySQL etc.) usage
- Tracing and/or custom metrics are not a must-have currently, but to have good SLIs (and dashboard) most likely they need to be implemented for those purposes
    - See [docs](https://docs.google.com/document/d/1IhxFN2zo3wmYv1dHfSEP1ZSLCKmHKOdVcOUka2TIf8k/edit#)

## Logging
- [X] Must log in JSON format and have at least the following as fields:
    - [X] Level (eg. Fatal, Error, Warning, Information, Debug, Verbose)
    - [X] Message
    - [X] Timestamp
- [ ] LogLevel must tunable via environment variable
- [X] Logs must exclude PII/PCI data
    - [X] Passwords, names, phone numbers, emails, PAN numbers, CVV and user logins
    - See  [data dictionary](http://knowledge.questrade.com/asset/439ec378-2f85-495e-815c-27d9c34df077)
- [X]  Must have ability to log every request
    - But should not have that enabled with Information or higher loglevels as Istio does raw request logging
- [X] All errors/exceptions must be caught and logged
- [X] Logs must follow [best practices](https://docs.google.com/document/d/1IhxFN2zo3wmYv1dHfSEP1ZSLCKmHKOdVcOUka2TIf8k/edit#heading=h.ewbkjuohju4w) and formatting standards
- [X] Team must have reviewed logging output in SIT to make sure its at the correct level
    - [X] Logging enough so that errors are caught
    - [X] Logging only things that have value
    - [X] Logging in a structure that is easy for both creating alerts and for humans to understand

## Application Security
- [X] Application must have been scanned via SonarQube and must have the SonarQube Dashboard link for the app available in the readme file. Any major findings must have been fixed or determined to be false positives.
- [ ] Application must have been scanned via relevant DAST tool (InsighAppSec for NowSecure) and must have the DAST tool link for the app available in the readme file. Any major findings must have been fixed or determined to be false positives.
