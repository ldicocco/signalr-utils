# CoffeeScript

_myConnection = null
_myMediaStream = null

hub = $.connection.webRTCHub
$.connection.hub.url = '/signalr/hubs'
$.connection.hub.start ->
    console.log 'connected to signal server.'
    init()

_createConnection = ->
    console.log('creating RTCPeerConnection...')
    connection = new RTCPeerConnection null # null = no ICE servers
    connection.onicecandidate = (event) ->
        if event.candidate?
            hub.server.send(JSON.stringify({ "candidate": event.candidate }))
        return
    connection.onaddstream = (event) ->
        newVideoElement = document.createElement('video')
        newVideoElement.className = 'video'
        newVideoElement.autoplay = 'autoplay'

        # Attach the stream to the Video element via adapter.js
        attachMediaStream newVideoElement, event.stream

        # Add the new Video element to the page
        document.querySelector('body').appendChild(newVideoElement)

        # Turn off the call button, since we should be in a call now
        document.querySelector('#startBtn').setAttribute('disabled', 'disabled')
        return
    connection

hub.client.newMessage = (data) ->
    message = JSON.parse(data)
    #alert data
    connection = _myConnection ? _createConnection(null)
    if message.sdp?
        connection.setRemoteDescription new RTCSessionDescription(message.sdp), ->
            if (connection.remoteDescription.type == 'offer')
                console.log('received offer, sending answer...')
                # Add our stream to the connection to be shared
                connection.addStream(_myMediaStream)
                # Create an SDP response
                connection.createAnswer (desc) ->
                    # Which becomes our local session description
                    connection.setLocalDescription desc, ->
                        # And send it to the originator, where it will become their RemoteDescription
                        hub.server.send(JSON.stringify({ 'sdp': connection.localDescription }))
            else if (connection.remoteDescription.type == 'answer')
                console.log 'got an answer'
            return
    else if message.candidate?
        console.log 'adding ice candidate...'
        connection.addIceCandidate (new RTCIceCandidate message.candidate)
    _myConnection = connection
    return

getUserMediaAsync = (config) ->
    d = $.Deferred()
    getUserMedia config,
        (stream) -> d.resolve stream

        (error) -> d.reject error
    
    return d.promise()

connectionCreateOfferAsync = (conn, desc) ->
    d = $.Deferred()
    conn.createOffer (desc) -> d.resolve(desc)
        
    return d.promise()

init = ->
    p = getUserMediaAsync video: true, audio: true
    p.done  (stream) ->
            videoElement = document.getElementById 'rtcVideo'
            _myMediaStream = stream
            attachMediaStream(videoElement, _myMediaStream)
            document.querySelector('#startBtn').removeAttribute('disabled')
            return
    p.fail  (error) ->
            alert(JSON.stringify(error)) 

    document.querySelector('#startBtn').addEventListener 'click', ->
        _myConnection ?= _createConnection(null)
        _myConnection.addStream _myMediaStream
        p1 = connectionCreateOfferAsync _myConnection
        p1.done (desc) ->
            _myConnection.setLocalDescription desc, ->
                hub.server.send JSON.stringify({ "sdp": desc })
                return
            return
        return
#alert "bip"
