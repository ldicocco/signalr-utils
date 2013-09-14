(function() {
  var getUserMediaAsync, hub, init, _createConnection, _myConnection, _myMediaStream;

  _myConnection = null;

  _myMediaStream = null;

  hub = $.connection.webRTCHub;

  $.connection.hub.url = '/signalr/hubs';

  $.connection.hub.start(function() {
    console.log('connected to signal server.');
    return init();
  });

  _createConnection = function() {
    var connection;
    console.log('creating RTCPeerConnection...');
    connection = new RTCPeerConnection(null);
    connection.onicecandidate = function(event) {
      if (event.candidate != null) {
        hub.server.send(JSON.stringify({
          "candidate": event.candidate
        }));
      }
    };
    connection.onaddstream = function(event) {
      var newVideoElement;
      newVideoElement = document.createElement('video');
      newVideoElement.className = 'video';
      newVideoElement.autoplay = 'autoplay';
      attachMediaStream(newVideoElement, event.stream);
      document.querySelector('body').appendChild(newVideoElement);
      document.querySelector('#startBtn').setAttribute('disabled', 'disabled');
    };
    return connection;
  };

  hub.client.newMessage = function(data) {
    var connection, message;
    message = JSON.parse(data);
    connection = _myConnection != null ? _myConnection : _createConnection(null);
    if (message.sdp != null) {
      connection.setRemoteDescription(new RTCSessionDescription(message.sdp), function() {
        if (connection.remoteDescription.type === 'offer') {
          console.log('received offer, sending answer...');
          connection.addStream(_myMediaStream);
          connection.createAnswer(function(desc) {
            return connection.setLocalDescription(desc, function() {
              return hub.server.send(JSON.stringify({
                'sdp': connection.localDescription
              }));
            });
          });
        } else if (connection.remoteDescription.type === 'answer') {
          console.log('got an answer');
        }
      });
    } else if (message.candidate != null) {
      console.log('adding ice candidate...');
      connection.addIceCandidate(new RTCIceCandidate(message.candidate));
    }
    _myConnection = connection;
  };

  getUserMediaAsync = function(config) {
    var d;
    d = $.Deferred();
    getUserMedia(config, function(stream) {
      return d.resolve(stream);
    }, function(error) {
      return d.reject(error);
    });
    return d.promise();
  };

  init = function() {
    var p;
    p = getUserMediaAsync({
      video: true,
      audio: true
    });
    p.done(function(stream) {
      var videoElement;
      videoElement = document.getElementById('rtcVideo');
      _myMediaStream = stream;
      attachMediaStream(videoElement, _myMediaStream);
      document.querySelector('#startBtn').removeAttribute('disabled');
    });
    p.fail(function(error) {
      return alert(JSON.stringify(error));
    });
    return document.querySelector('#startBtn').addEventListener('click', function() {
      if (_myConnection == null) {
        _myConnection = _createConnection(null);
      }
      _myConnection.addStream(_myMediaStream);
      _myConnection.createOffer(function(desc) {
        _myConnection.setLocalDescription(desc, function() {
          hub.server.send(JSON.stringify({
            "sdp": desc
          }));
        });
      });
    });
  };

}).call(this);
